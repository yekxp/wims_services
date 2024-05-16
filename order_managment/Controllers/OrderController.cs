using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using order_managment.Messaging;
using order_managment.Model;
using order_managment.Model.Contract;
using order_managment.Services;

namespace order_managment.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderServices _orderService;
        private readonly IMessagePublisher _messagePublisher;

        public OrderController(IMessagePublisher messageProducer, IOrderServices orderService)
        {

            _messagePublisher = messageProducer;
            _orderService = orderService;
        }

        [Authorize(Roles = "Customer, Admin")]
        [HttpPost("/createOrder")]
        public async Task<ActionResult<Order>> CreateOrder([FromBody] Order order)
        {
            Order createdOrder = await _orderService.CreateOrder(order);

            if (createdOrder is not null)
            {
                await _messagePublisher.PublishMessage(createdOrder);
            }

            return Ok(createdOrder);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("/updateStatusOrder/{id}/{status}")]
        public async Task<ActionResult<Order>> ChangeOrderStatus(string id, string status)
        {
            Order updatedOrder = await _orderService.ChangeOrderStatus(id, status);

            return Ok(updatedOrder);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("/getOrders")]
        public async Task<ActionResult<List<Order>>> GetOrders()
        {
            List<Order> orders = await _orderService.GetOrders();

            return Ok(orders);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("/getOrderById/{id}")]
        public async Task<ActionResult<Order>> GetOrder(string id)
        {
            Order order = await _orderService.GetOrderById(id);

            return Ok(order);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("/deleteOrder/{id}")]
        public async Task<ActionResult<Order>> DeleteOrder(string id)
        {
            await _orderService.DeleteOrder(id);

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("/getOrderInfo")]
        public async Task<ActionResult<OrdersAnalytics>> GetOrderInfo()
        {
            OrdersAnalytics order = await _orderService.GetOrderInfo();

            return Ok(order);
        }

        [Authorize(Roles = "Customer, Admin")]
        [HttpPost("/filter")]
        public async Task<ActionResult<List<Order>>> Filter(OrderFilter filterPayload)
        {
            List<Order> orders = await _orderService.OrderFilter(filterPayload);

            return Ok(orders);
        }

        
    }
}
