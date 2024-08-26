using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Domain/Entities/Entrega.cs
namespace DesafioRentDelivery.Domain.Entities
{
    public class Entrega
    {
        public int Id { get; set; }
        public int EntregadorId { get; set; }
        public DateTime DataEntrega { get; set; }
        public string Destino { get; set; }
        public string Status { get; set; }

        public Entregador Entregador { get; set; }
    }
}