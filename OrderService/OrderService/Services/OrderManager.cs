using OrderService.Models;
using OrderService.Data;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace OrderService.Services
{
    public class OrderManager
    {
        private readonly OrdersDbContext _context;
        private readonly IProductApiClient _productApiClient;

        public OrderManager(OrdersDbContext context, IProductApiClient productApiClient)
        {
            _context = context;
            _productApiClient = productApiClient;
        }

        public IEnumerable<Order> GetAll() => _context.Orders.ToList();

        public Order? GetById(Guid id) => _context.Orders.FirstOrDefault(o => o.Id == id);

        public async Task AddAsync(Order order)
        {
            // Validate product IDs
            bool areValid = await _productApiClient.AreProductIdsValidAsync(order.ProductIds);
            if (!areValid)
            {
                throw new ArgumentException("One or more product IDs are invalid.");
            }

            // Prepare order
            order.Id = Guid.NewGuid();
            order.CreatedAt = DateTime.UtcNow;

            // Save to DB
            _context.Orders.Add(order);
            _context.SaveChanges();

            // Publish to RabbitMQ
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "order-created",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var message = JsonSerializer.Serialize(order);
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "",
                                 routingKey: "order-created",
                                 basicProperties: null,
                                 body: body);
        }
    }
}
