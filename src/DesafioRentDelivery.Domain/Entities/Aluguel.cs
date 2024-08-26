using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Domain/Entities/Aluguel.cs
namespace DesafioRentDelivery.Domain.Entities
{
    public class Aluguel
    {
        public int Id { get; set; }
        public int EntregadorId { get; set; }
        public int MotoId { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }

        public Entregador Entregador { get; set; }
        public Moto Moto { get; set; }
    }
}