// Application/DTOs/EntregaDTO.cs
using System;

namespace DesafioRentDelivery.Application.DTOs
{
    public class EntregaDTO
    {
        public int Id { get; set; }
        public int EntregadorId { get; set; }
        public DateTime DataEntrega { get; set; }
        public string Destino { get; set; }
        public string Status { get; set; }
    }
}
