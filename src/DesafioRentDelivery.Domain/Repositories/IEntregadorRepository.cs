// Domain/Repositories/IEntregadorRepository.cs
using DesafioRentDelivery.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesafioRentDelivery.Domain.Repositories
{
    public interface IEntregadorRepository
    {
        Task AddEntregadorAsync(Entregador entregador);
        Task<Entregador> GetEntregadorByIdAsync(int id);
        Task<IEnumerable<Entregador>> GetAllEntregadoresAsync();
        Task UpdateEntregadorAsync(Entregador entregador);
        Task RemoveEntregadorAsync(Entregador entregador);
    }
}
