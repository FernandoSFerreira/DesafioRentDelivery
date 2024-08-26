// Domain/Repositories/IMotoRepository.cs
using DesafioRentDelivery.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesafioRentDelivery.Domain.Repositories
{
    public interface IMotoRepository
    {
        Task AddMotoAsync(Moto moto);
        Task<Moto> GetMotoByIdAsync(int id);
        Task<IEnumerable<Moto>> GetAllMotosAsync();
        Task UpdateMotoAsync(Moto moto);
        Task RemoveMotoAsync(Moto moto);

        Task AddHistoricoManutencaoAsync(HistoricoManutencao historico);
        Task RemoveHistoricoManutencaoAsync(HistoricoManutencao historico);
    }
}
