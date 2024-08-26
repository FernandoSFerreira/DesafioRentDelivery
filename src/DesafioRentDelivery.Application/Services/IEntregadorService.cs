// Application/Services/IEntregadorService.cs
using DesafioRentDelivery.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesafioRentDelivery.Application.Services
{
    public interface IEntregadorService
    {
        Task AddEntregadorAsync(EntregadorDTO entregadorDto);
        Task<EntregadorDTO> GetEntregadorByIdAsync(int id);
        Task<IEnumerable<EntregadorDTO>> GetAllEntregadoresAsync();
        Task UpdateEntregadorAsync(EntregadorDTO entregadorDto);
        Task RemoveEntregadorAsync(int id);
    }
}
