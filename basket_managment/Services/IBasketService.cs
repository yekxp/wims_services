using basket_managment.Model;

namespace basket_managment.Services
{
    public interface IBasketService
    {
        Task AddToBasket(BasketInfo product);

        Task<string> RemoveFromBasket(string productName, string buyerName);

        Task<BasketInfo?> GetBasket(string buyerName);
    }
}
