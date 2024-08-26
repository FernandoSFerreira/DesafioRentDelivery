// Application/DTOs/MotoDTO.cs
using System.Collections.Generic;

namespace DesafioRentDelivery.Application.DTOs
{
    public class MotoDTO
    {
        public int Id { get; set; }
        public string Placa { get; set; }
        public string Modelo { get; set; }
        public string Chassi { get; set; }
        public List<HistoricoManutencaoDTO> HistoricoManutencoes { get; set; }
    }
}
