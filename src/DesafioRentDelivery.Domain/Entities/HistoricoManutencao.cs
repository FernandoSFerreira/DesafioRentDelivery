// Domain/Entities/HistoricoManutencao.cs
using System;

namespace DesafioRentDelivery.Domain.Entities
{
    public class HistoricoManutencao
    {
        public int Id { get; set; }
        public int MotoId { get; set; }
        public DateTime DataManutencao { get; set; }
        public string Descricao { get; set; }

        public Moto Moto { get; set; }
    }
}
