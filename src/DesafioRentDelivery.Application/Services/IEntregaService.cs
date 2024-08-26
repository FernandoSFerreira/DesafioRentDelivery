// Application/Services/IEntregaService.cs
using DesafioRentDelivery.Application.DTOs;
using DesafioRentDelivery.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesafioRentDelivery.Application.Services
{
    public interface IEntregaService
    {
        Task AddEntregaAsync(EntregaDTO entregaDto);
        Task<EntregaDTO> GetEntregaByIdAsync(int id);
        Task<IEnumerable<EntregaDTO>> GetAllEntregasAsync();
        Task UpdateEntregaAsync(EntregaDTO entregaDto);
        Task RemoveEntregaAsync(int id);
    }
}
