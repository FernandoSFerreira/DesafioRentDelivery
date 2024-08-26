// Application/Services/IAluguelService.cs
using DesafioRentDelivery.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesafioRentDelivery.Application.Services
{
    public interface IAluguelService
    {
        Task AddAluguelAsync(AluguelDTO aluguelDto);
        Task<AluguelDTO> GetAluguelByIdAsync(int id);
        Task<IEnumerable<AluguelDTO>> GetAllAlugueisAsync();
        Task<IEnumerable<AluguelDTO>> GetAlugueisByEntregadorIdAsync(int entregadorId);
        Task<AluguelDTO> GetAluguelAtivoByEntregadorIdAsync(int entregadorId);
        Task UpdateAluguelAsync(AluguelDTO aluguelDto);
        Task RemoveAluguelAsync(int id);
    }
}
