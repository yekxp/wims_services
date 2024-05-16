using inventory_managment.Model;
using inventory_managment.Model.api;

namespace inventory_managment.Services
{
    public interface IWarehouseService
    {
        Task AddWarehouse(Warehouse warehouse);

        Task<List<Warehouse>> GetAllWarehouses();

        Task<Warehouse?> GetWarehouseById(string id);

        Task AddProductsToWarehouse(RequestProducts requestProducts);

        Task RemoveProductsFromWareHouse(RequestProducts requestProducts);

        Task UpdateProductInfo(RequestProduct requestProduct);

        Task<List<Product>> FilterProducts(FilterPayload payload);
    }
}
