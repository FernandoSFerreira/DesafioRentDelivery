// Domain/Repositories/IEntregaRepository.cs
using DesafioRentDelivery.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesafioRentDelivery.Domain.Repositories
{
    public interface IEntregaRepository
    {
        Task AddEntregaAsync(Entrega entrega);
        Task<Entrega> GetEntregaByIdAsync(int id);
        Task<IEnumerable<Entrega>> GetAllEntregasAsync();
        Task<IEnumerable<Entrega>> GetEntregasByEntregadorIdAsync(int entregadorId); // Adicionado
        Task UpdateEntregaAsync(Entrega entrega);
        Task RemoveEntregaAsync(Entrega entrega);
    }
}
