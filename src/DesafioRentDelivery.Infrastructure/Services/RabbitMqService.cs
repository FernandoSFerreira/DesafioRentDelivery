// Infrastructure/Services/RabbitMqService.cs
using DesafioRentDelivery.Infrastructure.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Text;

namespace DesafioRentDelivery.Infrastructure.Services
{
    public class RabbitMqService : IRabbitMqService, IDisposable
    {
        private readonly RabbitMqConfiguration _config;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ILogger<RabbitMqService> _logger;

        public RabbitMqService(IOptions<RabbitMqConfiguration> config, ILogger<RabbitMqService> logger)
        {
            _config = config.Value;
            _logger = logger;

            try
            {
                _logger.LogInformation("Initializing RabbitMQ connection and channel.");

                var factory = new ConnectionFactory
                {
                    HostName = _config.HostName,
                    UserName = _config.Username,
                    Password = _config.Password
                };

                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.QueueDeclare(queue: _config.QueueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                _logger.LogInformation("RabbitMQ connection and channel successfully initialized.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initializing RabbitMQ connection and channel.");
                throw;
            }
        }

        public void SendMessage(string message)
        {
            try
            {
                _logger.LogInformation("Sending message to RabbitMQ: {Message}", message);

                var body = Encoding.UTF8.GetBytes(message);

                _channel.BasicPublish(exchange: "",
                                      routingKey: _config.QueueName,
                                      basicProperties: null,
                                      body: body);

                _logger.LogInformation("Message successfully sent to RabbitMQ.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending message to RabbitMQ.");
                throw;
            }
        }

        // Limpeza dos recursos
        public void Dispose()
        {
            try
            {
                _logger.LogInformation("Disposing RabbitMQ resources.");

                _channel?.Close();
                _connection?.Close();

                _logger.LogInformation("RabbitMQ resources successfully disposed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while disposing RabbitMQ resources.");
            }
        }
    }
}
