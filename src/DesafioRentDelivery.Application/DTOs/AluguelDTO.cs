// Application/DTOs/AluguelDTO.cs
using System;

namespace DesafioRentDelivery.Application.DTOs
{
    public class AluguelDTO
    {
        public int Id { get; set; }
        public int EntregadorId { get; set; }
        public int MotoId { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
    }
}
