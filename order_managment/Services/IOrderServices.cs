using order_managment.Model;
using order_managment.Model.Contract;

namespace order_managment.Services
{
    public interface IOrderServices
    {
        Task<Order> CreateOrder(Order order);

        Task<Order> ChangeOrderStatus(string orderId, string status);

        Task<List<Order>> GetOrders();

        Task<Order> GetOrderById(string orderId);

        Task<OrdersAnalytics> GetOrderInfo();

        Task<List<Order>> OrderFilter(OrderFilter filterPayload);

        Task DeleteOrder(string id);
    }
}
