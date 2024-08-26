// Infrastructure/Repositories/MotoRepository.cs
using DesafioRentDelivery.Domain.Entities;
using DesafioRentDelivery.Domain.Repositories;
using DesafioRentDelivery.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesafioRentDelivery.Infrastructure.Repositories
{
    public class MotoRepository : IMotoRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MotoRepository> _logger;

        public MotoRepository(ApplicationDbContext context, ILogger<MotoRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddMotoAsync(Moto moto)
        {
            try
            {
                _logger.LogInformation("Adding a new moto with Placa: {Placa}", moto.Placa);
                await _context.Motos.AddAsync(moto);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Moto successfully added with ID: {Id}", moto.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a new moto with Placa: {Placa}", moto.Placa);
                throw;
            }
        }

        public async Task<Moto> GetMotoByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Fetching moto with ID: {Id}", id);
                var moto = await _context.Motos
                    .Include(m => m.HistoricoManutencoes)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (moto == null)
                {
                    _logger.LogWarning("Moto with ID: {Id} not found.", id);
                }

                return moto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching moto with ID: {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<Moto>> GetAllMotosAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all motos.");
                var motos = await _context.Motos
                    .Include(m => m.HistoricoManutencoes)
                    .ToListAsync();

                _logger.LogInformation("Successfully fetched {Count} motos.", motos.Count);

                return motos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all motos.");
                throw;
            }
        }

        public async Task UpdateMotoAsync(Moto moto)
        {
            try
            {
                _logger.LogInformation("Updating moto with ID: {Id}", moto.Id);
                _context.Motos.Update(moto);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Moto successfully updated with ID: {Id}", moto.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating moto with ID: {Id}", moto.Id);
                throw;
            }
        }

        public async Task RemoveMotoAsync(Moto moto)
        {
            try
            {
                _logger.LogInformation("Removing moto with ID: {Id}", moto.Id);
                _context.Motos.Remove(moto);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Moto successfully removed with ID: {Id}", moto.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while removing moto with ID: {Id}", moto.Id);
                throw;
            }
        }

        public async Task AddHistoricoManutencaoAsync(HistoricoManutencao historico)
        {
            try
            {
                _logger.LogInformation("Adding historico de manutencao for Moto ID: {MotoId}", historico.MotoId);
                await _context.HistoricoManutencoes.AddAsync(historico);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Historico de manutencao successfully added with ID: {Id}", historico.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding historico de manutencao for Moto ID: {MotoId}", historico.MotoId);
                throw;
            }
        }

        public async Task RemoveHistoricoManutencaoAsync(HistoricoManutencao historico)
        {
            try
            {
                _logger.LogInformation("Removing historico de manutencao with ID: {Id}", historico.Id);
                _context.HistoricoManutencoes.Remove(historico);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Historico de manutencao successfully removed with ID: {Id}", historico.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while removing historico de manutencao with ID: {Id}", historico.Id);
                throw;
            }
        }
    }
}
