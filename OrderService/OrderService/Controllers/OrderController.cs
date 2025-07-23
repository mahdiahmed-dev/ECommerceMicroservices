using Microsoft.AspNetCore.Mvc;
using OrderService.Models;
using OrderService.Services;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly OrderManager _manager;

        public OrderController(OrderManager manager)
        {
            _manager = manager;
        }

        [HttpGet]
        public IActionResult Get() => Ok(_manager.GetAll());

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var order = _manager.GetById(id);
            return order is null ? NotFound() : Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Order order)
        {
            if (order == null)
                return BadRequest("Order is null.");

            await _manager.AddAsync(order);

            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }
    }
}
