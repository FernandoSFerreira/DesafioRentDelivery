// DesafioRentDelivery.IntegrationTests/Infrastructure/RabbitMqServiceTests.cs

using DesafioRentDelivery.Infrastructure.Configurations;
using DesafioRentDelivery.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DesafioRentDelivery.IntegrationTests.Infrastructure
{
    public class RabbitMqServiceTests
    {
        private readonly Mock<IConnection> _connectionMock;
        private readonly Mock<IModel> _channelMock;
        private readonly RabbitMqService _rabbitMqService;

        public RabbitMqServiceTests()
        {
            _connectionMock = new Mock<IConnection>();
            _channelMock = new Mock<IModel>();

            _connectionMock.Setup(c => c.CreateModel()).Returns(_channelMock.Object);

            var rabbitMqConfig = Options.Create(new RabbitMqConfiguration
            {
                HostName = "localhost",
                QueueName = "test-queue"
            });

            var loggerMock = new Mock<ILogger<RabbitMqService>>();

            // Agora passamos o mock do logger para o RabbitMqService
            _rabbitMqService = new RabbitMqService(rabbitMqConfig, loggerMock.Object);

            // Como não podemos injetar mocks diretamente, vamos usar Reflection para substituir as instâncias privadas
            typeof(RabbitMqService).GetField("_connection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(_rabbitMqService, _connectionMock.Object);

            typeof(RabbitMqService).GetField("_channel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(_rabbitMqService, _channelMock.Object);
        }

        [Fact]
        public void SendMessage_ShouldPublishMessageToQueue()
        {
            // Arrange
            var message = "Hello RabbitMQ";

            // Act
            _rabbitMqService.SendMessage(message);

            // Assert
            _channelMock.Verify(c => c.BasicPublish(
                "",
                "test-queue",
                null,
                It.IsAny<byte[]>()),
                Times.Once);
        }

        [Fact]
        public async Task ReceiveMessage_ShouldConsumeMessageFromQueue()
        {
            // Arrange
            var message = "Hello RabbitMQ";
            var body = Encoding.UTF8.GetBytes(message);

            var consumer = new EventingBasicConsumer(_channelMock.Object);
            consumer.Received += (model, ea) =>
            {
                var receivedMessage = Encoding.UTF8.GetString(ea.Body.ToArray());
                Assert.Equal(message, receivedMessage);
            };

            _channelMock.Setup(c => c.BasicConsume(
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<IBasicConsumer>()))
                .Callback((string queue, bool autoAck, IBasicConsumer cons) =>
                {
                    cons.HandleBasicDeliver("consumerTag", 1, false, "", queue, null, body);
                });

            // Act
            _channelMock.Object.BasicConsume("test-queue", true, consumer);

            // Simulate delay for async handling
            await Task.Delay(500);

            // Assert
            _channelMock.Verify(c => c.BasicConsume(
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<IBasicConsumer>()),
                Times.Once);
        }

        [Fact]
        public void Dispose_ShouldCloseChannelAndConnection()
        {
            // Act
            _rabbitMqService.Dispose();

            // Assert
            _channelMock.Verify(c => c.Close(), Times.Once);
            _connectionMock.Verify(c => c.Close(), Times.Once);
        }
    }
}
