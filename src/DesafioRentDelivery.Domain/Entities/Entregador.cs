using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Domain/Entities/Entregador.cs
namespace DesafioRentDelivery.Domain.Entities
{
    public class Entregador
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Documento { get; set; }
        public string Telefone { get; set; }

        public ICollection<Aluguel> Alugueis { get; set; } = new List<Aluguel>();
        public ICollection<Entrega> Entregas { get; set; } = new List<Entrega>();
    }
}