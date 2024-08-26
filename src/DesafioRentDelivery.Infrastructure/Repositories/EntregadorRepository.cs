// Infrastructure/Repositories/EntregadorRepository.cs
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
    public class EntregadorRepository : IEntregadorRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EntregadorRepository> _logger;

        public EntregadorRepository(ApplicationDbContext context, ILogger<EntregadorRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Adiciona um novo entregador ao banco de dados
        public async Task AddEntregadorAsync(Entregador entregador)
        {
            try
            {
                _logger.LogInformation("Adding a new entregador with Documento: {Documento}", entregador.Documento);
                await _context.Entregadores.AddAsync(entregador);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Entregador successfully added with ID: {Id}", entregador.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a new entregador with Documento: {Documento}", entregador.Documento);
                throw;
            }
        }

        // Obtém um entregador por ID
        public async Task<Entregador> GetEntregadorByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Fetching entregador with ID: {Id}", id);
                var entregador = await _context.Entregadores
                    .Include(e => e.Alugueis)
                    .Include(e => e.Entregas)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (entregador == null)
                {
                    _logger.LogWarning("Entregador with ID: {Id} not found.", id);
                }

                return entregador;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching entregador with ID: {Id}", id);
                throw;
            }
        }

        // Obtém todos os entregadores
        public async Task<IEnumerable<Entregador>> GetAllEntregadoresAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all entregadores.");
                var entregadores = await _context.Entregadores
                    .Include(e => e.Alugueis)
                    .Include(e => e.Entregas)
                    .ToListAsync();

                _logger.LogInformation("Successfully fetched {Count} entregadores.", entregadores.Count);

                return entregadores;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all entregadores.");
                throw;
            }
        }

        // Atualiza um entregador existente
        public async Task UpdateEntregadorAsync(Entregador entregador)
        {
            try
            {
                _logger.LogInformation("Updating entregador with ID: {Id}", entregador.Id);
                _context.Entregadores.Update(entregador);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Entregador successfully updated with ID: {Id}", entregador.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating entregador with ID: {Id}", entregador.Id);
                throw;
            }
        }

        // Remove um entregador do banco de dados
        public async Task RemoveEntregadorAsync(Entregador entregador)
        {
            try
            {
                _logger.LogInformation("Removing entregador with ID: {Id}", entregador.Id);
                _context.Entregadores.Remove(entregador);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Entregador successfully removed with ID: {Id}", entregador.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while removing entregador with ID: {Id}", entregador.Id);
                throw;
            }
        }
    }
}
