// Domain/Entities/Moto.cs
using System.Collections.Generic;

namespace DesafioRentDelivery.Domain.Entities
{
    public class Moto
    {
        public int Id { get; set; }
        public string Placa { get; set; }
        public string Modelo { get; set; }
        public string Chassi { get; set; }

        public ICollection<HistoricoManutencao> HistoricoManutencoes { get; set; } = new List<HistoricoManutencao>();
    }
}
