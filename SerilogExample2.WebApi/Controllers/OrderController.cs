using Microsoft.AspNetCore.Mvc;
using SerilogExample2.WebApi.Services;

namespace SerilogExample2.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("process/{orderId}")]
        public IActionResult ProcessOrder(int orderId, string customerName)
        {
            _orderService.ProcessOrder(orderId, customerName);
            
            return Ok($"Order {orderId} processed successfully");
        }

        [HttpPost("cancel/{orderId}")]
        public IActionResult CancelOrder(int orderId, string reason)
        {
            _orderService.CancelOrder(orderId, reason);
            return Ok($"Order {orderId} cancel successfully");
        }
    }
}
