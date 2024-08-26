// Domain/Repositories/IAluguelRepository.cs
using DesafioRentDelivery.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesafioRentDelivery.Domain.Repositories
{
    public interface IAluguelRepository
    {
        Task AddAluguelAsync(Aluguel aluguel);
        Task<Aluguel> GetAluguelByIdAsync(int id);
        Task<IEnumerable<Aluguel>> GetAllAlugueisAsync();
        Task<IEnumerable<Aluguel>> GetAlugueisByEntregadorIdAsync(int entregadorId);
        Task<Aluguel> GetAluguelAtivoByEntregadorIdAsync(int entregadorId);
        Task UpdateAluguelAsync(Aluguel aluguel);
        Task RemoveAluguelAsync(Aluguel aluguel);
    }
}
