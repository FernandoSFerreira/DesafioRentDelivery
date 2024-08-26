// Infrastructure/Services/IRabbitMqService.cs
namespace DesafioRentDelivery.Infrastructure.Services
{
    public interface IRabbitMqService
    {
        void SendMessage(string message);
    }
}
