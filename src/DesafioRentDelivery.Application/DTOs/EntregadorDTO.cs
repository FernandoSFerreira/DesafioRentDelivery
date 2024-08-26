// Application/DTOs/EntregadorDTO.cs
using DesafioRentDelivery.Domain.Entities;
using System.Collections.Generic;

namespace DesafioRentDelivery.Application.DTOs
{
    public class EntregadorDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Documento { get; set; }
        public string Telefone { get; set; }

        // Dados adicionais para exibição de aluguéis e entregas
        public List<AluguelDTO> Alugueis { get; set; } = new List<AluguelDTO>();
        public List<EntregaDTO> Entregas { get; set; } = new List<EntregaDTO>();
    }
}
