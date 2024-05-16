using basket_managment.Model;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace basket_managment.Services
{
    public class BasketService(IDistributedCache distributedCache) : IBasketService
    {
        private readonly IDistributedCache _distributedCache = distributedCache;

        public async Task AddToBasket(BasketInfo basketInfo)
        {
            string? currentCache = await _distributedCache.GetStringAsync("BASKET");

            if (currentCache is null)
            {
                List<BasketInfo> currentProductsInCache = new() { basketInfo };

                string jsonString = JsonSerializer.Serialize(basketInfo);
                DistributedCacheEntryOptions cacheOptions = new DistributedCacheEntryOptions();
                cacheOptions.SetAbsoluteExpiration(new TimeSpan(0, 20, 0));
                await _distributedCache.SetStringAsync("BASKET", jsonString, cacheOptions);
            }
            else
            {
                BasketInfo currentProductsInCache = JsonSerializer.Deserialize<BasketInfo>(currentCache)!;

                var existingProduct = currentProductsInCache.Products.Where(p => p.Name == basketInfo.Products[0].Name);
                if (existingProduct != null)
                {
                    var existingProductInBasket = existingProduct.FirstOrDefault(prod => prod.Name == basketInfo.Products[0].Name);
                    if (existingProductInBasket != null)
                    {
                        existingProductInBasket.Quantity += basketInfo.Products[0].Quantity;
                    }
                    else
                    {
                        currentProductsInCache.Products.Add(basketInfo.Products[0]);
                    }
                }
                else
                {
                    currentProductsInCache.Products.AddRange(basketInfo.Products);
                }

                string jsonString = JsonSerializer.Serialize(currentProductsInCache);
                await _distributedCache.SetStringAsync("BASKET", jsonString);
            }
        }



        public async Task<BasketInfo?> GetBasket(string buyerName)
        {
            string? currentCache = await _distributedCache.GetStringAsync(buyerName);

            if (currentCache is null)
                return null;


            BasketInfo product = JsonSerializer.Deserialize<BasketInfo>(currentCache)!;

            return product;
        }

        public async Task<string> RemoveFromBasket(string productId, string buyerName)
        {
            string? currentCache = await _distributedCache.GetStringAsync(buyerName);

            if (currentCache is null)
            {
                return "Basket not found in cache.";
            }

            BasketInfo currentProductsInCache = JsonSerializer.Deserialize<BasketInfo>(currentCache)!;
            Product? producToRemove = currentProductsInCache.Products.FirstOrDefault(product => product.Id == productId);

            if (producToRemove == null)
            {
                return "Product not found in basket.";
            }

            var productToRemove = currentProductsInCache.Products.FirstOrDefault(product => product.Id == producToRemove.Id);
            currentProductsInCache.Products.Remove(productToRemove);

            string jsonString = JsonSerializer.Serialize(currentProductsInCache);
            await _distributedCache.SetStringAsync(buyerName, jsonString);

            return "cached";
        }

    }
}
