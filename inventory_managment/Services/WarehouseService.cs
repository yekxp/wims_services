using inventory_managment.Model;
using inventory_managment.Model.api;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.IdentityModel.Tokens;
using System.Net;

namespace inventory_managment.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly Container _container;

        public WarehouseService(CosmosClient cosmosClient, IConfiguration configuration)
        {
            Database database = cosmosClient.GetDatabase(configuration["CosmosDbSettings:DatabaseName"]);
            _container = database.CreateContainerIfNotExistsAsync("IM_Warehouse", "/warehouseId").Result;
        }

        public async Task AddProductsToWarehouse(RequestProducts requestProducts)
        {
            if (requestProducts == null)
            {
                throw new ArgumentNullException(nameof(requestProducts));
            }

            // Retrieve the warehouse by ID
            Warehouse warehouse = await _container.ReadItemAsync<Warehouse>(requestProducts.WarehouseId, new PartitionKey(requestProducts.WarehouseId));

            if (warehouse == null)
            {
                throw new ArgumentException($"Warehouse with ID {requestProducts.WarehouseId} not found.");
            }

            if (warehouse.Products is null)
            {
                warehouse.Products = [];
            }

            // Add products to the warehouse
            if (requestProducts.Products != null && requestProducts.Products.Any())
            {
                // Assuming warehouse.Products is not null
                foreach (var product in requestProducts.Products)
                {
                    // Check if the product already exists in the warehouse, if so update it, otherwise add it
                    var existingProduct = warehouse.Products.FirstOrDefault(p => p.Id == product.Id);
                    if (existingProduct != null)
                    {
                        // Update existing product
                        existingProduct.Name = product.Name;
                        existingProduct.Quantity += product.Quantity;
                    }
                    else
                    {
                        // Add new product to warehouse
                        warehouse.Products.Add(product);
                    }
                }

                // Update the warehouse in the database
                await _container.UpsertItemAsync(warehouse);
            }
        }

        public async Task AddWarehouse(Warehouse warehouse)
        {
            await _container.CreateItemAsync(warehouse);
        }

        public async Task<List<Warehouse>> GetAllWarehouses()
        {
            var items = _container.GetItemLinqQueryable<Warehouse>().ToFeedIterator();

            List<Warehouse> warehouses = new List<Warehouse>();

            if (items.HasMoreResults)
            {
                var result = await items.ReadNextAsync();
                warehouses.AddRange(result);
            }

            return warehouses;
        }

        public async Task<Warehouse?> GetWarehouseById(string id)
        {
            var query = _container.GetItemLinqQueryable<Warehouse>()
                        .Where(p => p.Id == id)
                        .Take(1)
                        .ToFeedIterator();

            var result = await query.ReadNextAsync();

            return result.FirstOrDefault();
        }

        public async Task UpdateProductInfo(RequestProduct requestProduct)
        {
            var warehouse = await GetWarehouseById(requestProduct.WarehouseId)
                ?? throw new Exception("Warehouse not found");

            var productToUpdate = warehouse.Products.FirstOrDefault(p => p.Id == requestProduct.Product.Id)
                ?? throw new Exception("Product not found in the warehouse");

            if (requestProduct.Product.Name != null)
            {
                productToUpdate.Name = requestProduct.Product.Name;
            }
            if (requestProduct.Product.Description != null)
            {
                productToUpdate.Description = requestProduct.Product.Description;
            }
            if (requestProduct.Product.CategoryId != null)
            {
                productToUpdate.CategoryId = requestProduct.Product.CategoryId;
            }
            if (requestProduct.Product.Price != -1)
            {
                productToUpdate.Price = requestProduct.Product.Price;
            }
            if (requestProduct.Product.Quantity != -1)
            {
                productToUpdate.Quantity = requestProduct.Product.Quantity;
            }

            await UpdateWarehouse(warehouse);
        }


        private async Task UpdateWarehouse(Warehouse warehouse)
        {
            try
            {
                var response = await _container.ReplaceItemAsync(
                    warehouse,
                    warehouse.Id,
                    new PartitionKey(warehouse.WarehouseId));

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception($"Failed to update warehouse. Status code: {response.StatusCode}");
                }
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                throw new Exception("Warehouse not found", ex);
            }
            catch (CosmosException ex)
            {
                throw new Exception("Failed to update warehouse", ex);
            }
        }

        public async Task RemoveProductsFromWareHouse(RequestProducts requestProducts)
        {
            var warehouse = await GetWarehouseById(requestProducts.WarehouseId) ?? throw new Exception("Warehouse not found");

            foreach (var product in requestProducts.Products)
            {
                var productToRemove = warehouse.Products.FirstOrDefault(p => p.Id == product.Id);
                if (productToRemove != null)
                {
                    warehouse.Products.Remove(productToRemove);
                }
            }

            await UpdateWarehouse(warehouse);
        }

        public async Task<List<Product>> FilterProducts(FilterPayload payload)
        {
            List<Product> filteredProducts = new List<Product>();

            // Retrieve all warehouses if WarehouseId is not set
            List<Warehouse> warehouses = new List<Warehouse>();
            if (payload.WarehouseId == null || !payload.WarehouseId.Any())
            {
                warehouses = await GetAllWarehouses();
            }
            else
            {
                foreach(var warehouseId in payload.WarehouseId)
                {
                   var warehouse = await GetWarehouseById(warehouseId);
                    if (warehouse is not null)
                        warehouses.Add(warehouse);
                }
            }

            foreach (var warehouse in warehouses)
            {
                var productsToFilter = (payload.CategoryId == null || !payload.CategoryId.Any())
                    ? warehouse.Products
                    : warehouse.Products.Where(p => payload.CategoryId.Contains(p.CategoryId)).ToList();

                // Apply price filter if provided
                if (!string.IsNullOrEmpty(payload.Price))
                {
                    productsToFilter = FilterByPrice(productsToFilter, payload.Price).ToList();
                }

                // Apply quantity filter if provided
                if (!string.IsNullOrEmpty(payload.Quantity))
                {
                    productsToFilter = FilterByQuantity(productsToFilter, payload.Quantity).ToList();
                }

                // Add filtered products to the result list
                filteredProducts.AddRange(productsToFilter);
            }

            return filteredProducts;
        }

        private IEnumerable<Product> FilterByPrice(IEnumerable<Product> products, string priceFilter)
        {
            // Parse the price filter string and extract the operator and value
            string[] parts = priceFilter.Split(new[] { "<=", ">=", "<", ">" }, StringSplitOptions.RemoveEmptyEntries);
            string comparisonOperator = priceFilter.Replace(parts[0], "").Trim(); // Extract the comparison operator
            decimal value = decimal.Parse(parts[0]);

            // Filter products based on the specified price condition
            switch (comparisonOperator)
            {
                case "<":
                    return products.Where(p => p.Price < value);
                case ">":
                    return products.Where(p => p.Price > value);
                case "<=":
                    return products.Where(p => p.Price <= value);
                case ">=":
                    return products.Where(p => p.Price >= value);
                default:
                    throw new ArgumentException("Invalid price filter");
            }
        }
        private IEnumerable<Product> FilterByQuantity(IEnumerable<Product> products, string quantityFilter)
        {
            // Parse the quantity filter string and extract the operator and value
            string[] parts = quantityFilter.Split(new[] { "<=", ">=", "<", ">" }, StringSplitOptions.RemoveEmptyEntries);
            string comparisonOperator = quantityFilter.Replace(parts[0], "").Trim(); // Extract the comparison operator
            int value = int.Parse(parts[0]);

            // Filter products based on the specified quantity condition
            switch (comparisonOperator)
            {
                case "<":
                    return products.Where(p => p.Quantity < value);
                case ">":
                    return products.Where(p => p.Quantity > value);
                case "<=":
                    return products.Where(p => p.Quantity <= value);
                case ">=":
                    return products.Where(p => p.Quantity >= value);
                default:
                    throw new ArgumentException("Invalid quantity filter");
            }
        }


    }
}
