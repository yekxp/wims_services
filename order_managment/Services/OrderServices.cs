using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Caching.Distributed;
using order_managment.Model;
using order_managment.Model.Contract;

namespace order_managment.Services
{
    public class OrderServices : IOrderServices
    {
        private readonly ILogger<OrderServices> _logger;
        private readonly Container _containerOrder;
        private readonly Container _containerOrderMetadata;
        private readonly IDistributedCache _distributedCache;

        public OrderServices(ILogger<OrderServices> logger, CosmosClient cosmosClient, IConfiguration configuration, IDistributedCache cache)
        {
            _logger = logger;
            Database database = cosmosClient.GetDatabase(configuration["CosmosDbSettings:DatabaseName"]);
            _containerOrder = database.CreateContainerIfNotExistsAsync("OM_Order", "/orderId").Result;
            _containerOrderMetadata = database.CreateContainerIfNotExistsAsync("OM_OrderMetadata", "/metadataId").Result;
            _distributedCache = cache;
        }

        public async Task<Order> ChangeOrderStatus(string orderId, string status)
        {
            var query = _containerOrder.GetItemLinqQueryable<Order>()
                .Where(p => p.OrderId == orderId)
                .Take(1)
                .ToFeedIterator();

            var result = await query.ReadNextAsync();

            Order? order = result.FirstOrDefault();

            order!.Status = status;

            await _containerOrder.ReplaceItemAsync(order, orderId, new PartitionKey(orderId));

            return order;
        }

        public async Task<Order> CreateOrder(Order order)
        {
            order.Price = order.OrderItems.Sum(p => (p.Price * p.Quantity));

            await _containerOrder.CreateItemAsync(order);
            await _distributedCache.RemoveAsync("BASKET");

            var query = _containerOrderMetadata.GetItemLinqQueryable<OrdersAnalytics>()
                        .Take(1)
                        .ToFeedIterator();

            var result = await query.ReadNextAsync();

            OrdersAnalytics? ordersAnalytics = result.FirstOrDefault();


            if (ordersAnalytics is null)
            {
                ordersAnalytics = new OrdersAnalytics();
                ordersAnalytics.AddOrder(order.CreatedAt, order.Price);
                await _containerOrderMetadata.CreateItemAsync(ordersAnalytics);
            }
            else
            {
                ordersAnalytics.AddOrder(order.CreatedAt, order.Price);
                await _containerOrderMetadata.UpsertItemAsync(ordersAnalytics);
            }

            _logger.LogDebug("Successufully created order!");

            return order;
        }

        public async Task<OrdersAnalytics> GetOrderInfo()
        {
            var query = _containerOrderMetadata.GetItemLinqQueryable<OrdersAnalytics>()
                        .Take(1)
                        .ToFeedIterator();

            var result = await query.ReadNextAsync();

            OrdersAnalytics? ordersAnalytics = result.FirstOrDefault();

            return ordersAnalytics;
        }

        public async Task<Order> GetOrderById(string orderId)
        {
            var query = _containerOrder.GetItemLinqQueryable<Order>()
                .Where(p => p.OrderId == orderId)
                .Take(1)
                .ToFeedIterator();

            var result = await query.ReadNextAsync();

            Order? order = result.FirstOrDefault();

            return order;
        }

        public async Task DeleteOrder(string id)
        {
            Order? order = await GetOrderById(id);

            if (order is not null)
                await _containerOrder.DeleteItemAsync<Order>(order.Id, new PartitionKey(order.OrderId));
        }

        public async Task<List<Order>> GetOrders()
        {
            var query = _containerOrder.GetItemLinqQueryable<Order>()
                .ToFeedIterator();

            List<Order> orders = [];

            if (query.HasMoreResults)
            {
                var result = await query.ReadNextAsync();
                orders.AddRange(result);
            }
            
            return orders;
        }

        public async Task<List<Order>> OrderFilter(OrderFilter filterPayload)
        {
            var query = _containerOrder.GetItemLinqQueryable<Order>();

            if (!string.IsNullOrEmpty(filterPayload.BuyerFullName))
            {
                var myNames = filterPayload.BuyerFullName!.Split(" ");
                query = (IOrderedQueryable<Order>)query.Where(p => p.Buyer.BuyerName == myNames[0] && p.Buyer.BuyerSurname == myNames[1]);
            }

            if (filterPayload.SortCreatedAt)
            {
                query = query.OrderBy(p => p.CreatedAt);
            }
            else if (filterPayload.SortCreatedAtDescending)
            {
                query = query.OrderByDescending(p => p.CreatedAt);
            }

            if (filterPayload.SortPrice)
            {
                query = query.OrderBy(p => p.Price);
            }
            else if (filterPayload.SortPriceDescending)
            {
                query = query.OrderByDescending(p => p.Price);
            }

            if (!string.IsNullOrEmpty(filterPayload.Price))
            {
                query = (IOrderedQueryable<Order>)query.Where(p => FilterByPrice(p.Price, filterPayload.Price));
            }

            var iterator = query.ToFeedIterator<Order>();
            var orders = new List<Order>();

            while (iterator.HasMoreResults)
            {
                var result = await iterator.ReadNextAsync();
                orders.AddRange(result);
            }

            return orders;
        }

        private bool FilterByPrice(decimal orderPrice, string priceFilter)
        {
            string[] parts = priceFilter.Split(new[] { "<=", ">=", "<", ">" }, StringSplitOptions.RemoveEmptyEntries);
            string comparisonOperator = priceFilter.Replace(parts[0], "").Trim();
            decimal value = decimal.Parse(parts[0]);

            switch (comparisonOperator)
            {
                case "<":
                    return orderPrice < value;
                case ">":
                    return orderPrice > value;
                case "<=":
                    return orderPrice <= value;
                case ">=":
                    return orderPrice >= value;
                default:
                    throw new ArgumentException("Invalid price filter");
            }
        }
    }
}
