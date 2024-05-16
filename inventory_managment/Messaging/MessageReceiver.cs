using Azure.Messaging.ServiceBus;
using inventory_managment.Model;
using inventory_managment.Model.api;
using inventory_managment.Model.message;
using inventory_managment.Services;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System.Text;

namespace inventory_managment.Messaging
{
    public class MessageReceiver : BackgroundService
    {
        private readonly ISubscriptionClient _subscriptionClient;
        private readonly IServiceProvider _serviceProvider;

        public MessageReceiver(ISubscriptionClient subscriptionClient, IServiceProvider serviceProvider)
        {
            _subscriptionClient = subscriptionClient;
            _serviceProvider = serviceProvider;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _subscriptionClient.RegisterMessageHandler((message, token) =>
            {
                List<OrderItem> orderConsumers = (JsonConvert.DeserializeObject<Order>(Encoding.UTF8.GetString(message.Body))!).OrderItems;

                using (var scope = _serviceProvider.CreateScope())
                {
                    IWarehouseService warehouseService = scope.ServiceProvider.GetRequiredService<IWarehouseService>();
                    List<Warehouse> warehouses = warehouseService.GetAllWarehouses().Result;

                    foreach (var warehouse in warehouses)
                    {
                        var productToUpdate = warehouse.Products.FirstOrDefault(p => orderConsumers.Any(oc => oc.Id == p.Id));

                        if (productToUpdate != null)
                        {
                            var correspondingOrderConsumer = orderConsumers.FirstOrDefault(oc => oc.Id == productToUpdate.Id);
                            if (correspondingOrderConsumer != null)
                            {
                                productToUpdate.Quantity -= correspondingOrderConsumer.Quantity;
                                if (productToUpdate.Quantity < 0)
                                    productToUpdate.Quantity = 0;
                            }

                            RequestProduct requestProduct = new RequestProduct
                            {
                                WarehouseId = warehouse.Id,
                                Product = new ProductUpdate
                                {
                                    Id = productToUpdate.Id,
                                    Quantity = productToUpdate.Quantity
                                }
                            };

                            warehouseService.UpdateProductInfo(requestProduct);
                            break;
                        }
                    }
                }

                return _subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
            }, new MessageHandlerOptions(args => Task.CompletedTask)
            {
                AutoComplete = false,
                MaxConcurrentCalls = 1
            });
            
            return Task.CompletedTask;
        }
    }
}
