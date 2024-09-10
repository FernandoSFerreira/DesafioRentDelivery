// Infrastructure/Services/IRabbitMqService.cs
namespace DesafioRentDelivery.Infrastructure.Services
{
    public interface IRabbitMqService
    {
        public void SendMessage(string message);
        public void Dispose();
    }
}
