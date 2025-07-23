using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using NotificationService.Models; // Adjust based on where NotificationEntry is defined


namespace NotificationService.Services
{
    public class RabbitMqListener : BackgroundService
    {
        private readonly ILogger<RabbitMqListener> _logger;
        private readonly NotificationRepository _repository;
        private IConnection _connection;
        private IModel _channel;

        public RabbitMqListener(ILogger<RabbitMqListener> logger, NotificationRepository repository)
        {
            _logger = logger;
            _repository = repository;

            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: "order-created",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                _logger.LogInformation($"Notification Received: {message}");

                try
                {
                    var notification = JsonSerializer.Deserialize<NotificationEntry>(message);

                    if (notification != null)
                    {
                        notification.Message = "New order placed at " + DateTime.UtcNow;
                        await _repository.AddAsync(notification);
                    }
                    else
                    {
                        _logger.LogWarning("Deserialized notification is null.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to deserialize or save notification");
                }
            };

            _channel.BasicConsume(queue: "order-created", autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
