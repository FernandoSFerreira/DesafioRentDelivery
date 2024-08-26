// Application/DTOs/HistoricoManutencaoDTO.cs
using System;

namespace DesafioRentDelivery.Application.DTOs
{
    public class HistoricoManutencaoDTO
    {
        public int Id { get; set; }
        public DateTime DataManutencao { get; set; }
        public string Descricao { get; set; }
    }
}
