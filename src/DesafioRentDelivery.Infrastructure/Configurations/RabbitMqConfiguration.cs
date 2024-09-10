using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Infrastructure/Configurations/RabbitMqConfiguration.cs
namespace DesafioRentDelivery.Infrastructure.Configurations
{
    public class RabbitMqConfiguration
    {
        public string HostName { get; set; } = "localhost";
        public string QueueName { get; set; } = "rent_delivery_queue";
        public string Username { get; set; } = "guest";
        public string Password { get; set; } = "guest";
    }
}