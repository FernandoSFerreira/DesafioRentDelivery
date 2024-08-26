// Infrastructure/Repositories/EntregaRepository.cs
using DesafioRentDelivery.Domain.Entities;
using DesafioRentDelivery.Domain.Repositories;
using DesafioRentDelivery.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DesafioRentDelivery.Infrastructure.Repositories
{
    public class EntregaRepository : IEntregaRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMongoCollection<Entrega> _mongoCollection;
        private readonly ILogger<EntregaRepository> _logger;

        public EntregaRepository(ApplicationDbContext context, IMongoClient mongoClient, ILogger<EntregaRepository> logger)
        {
            _context = context;
            _logger = logger;

            var database = mongoClient.GetDatabase("RentDeliveryDb");
            _mongoCollection = database.GetCollection<Entrega>("Entregas");
        }

        public async Task AddEntregaAsync(Entrega entrega)
        {
            try
            {
                _logger.LogInformation("Adding a new entrega with Entregador ID: {EntregadorId}", entrega.EntregadorId);

                // Adiciona no PostgreSQL
                await _context.Entregas.AddAsync(entrega);
                await _context.SaveChangesAsync();

                // Adiciona no MongoDB
                await _mongoCollection.InsertOneAsync(entrega);

                _logger.LogInformation("Entrega successfully added with ID: {Id}", entrega.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a new entrega.");
                throw;
            }
        }

        public async Task<Entrega> GetEntregaByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Fetching entrega with ID: {Id}", id);

                // Tenta obter a entrega do PostgreSQL
                var entrega = await _context.Entregas
                    .Include(e => e.Entregador)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (entrega == null)
                {
                    _logger.LogWarning("Entrega with ID: {Id} not found in PostgreSQL, checking MongoDB.", id);

                    // Se não encontrar no PostgreSQL, tenta obter do MongoDB
                    var filter = Builders<Entrega>.Filter.Eq("Id", id);
                    entrega = await _mongoCollection.Find(filter).FirstOrDefaultAsync();

                    if (entrega == null)
                    {
                        _logger.LogWarning("Entrega with ID: {Id} not found in MongoDB.", id);
                    }
                }

                return entrega;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching entrega with ID: {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<Entrega>> GetAllEntregasAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all entregas from PostgreSQL and MongoDB.");

                // Obtém todas as entregas do PostgreSQL
                var entregasSql = await _context.Entregas
                    .Include(e => e.Entregador)
                    .ToListAsync();

                // Obtém todas as entregas do MongoDB
                var entregasMongo = await _mongoCollection.Find(_ => true).ToListAsync();

                // Retorna a união de ambos
                var result = entregasSql.Union(entregasMongo).ToList();

                _logger.LogInformation("Successfully fetched {Count} entregas.", result.Count);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all entregas.");
                throw;
            }
        }

        public async Task<IEnumerable<Entrega>> GetEntregasByEntregadorIdAsync(int entregadorId)
        {
            try
            {
                _logger.LogInformation("Fetching entregas for Entregador ID: {EntregadorId} from PostgreSQL and MongoDB.", entregadorId);

                // Obtém as entregas do PostgreSQL
                var entregasSql = await _context.Entregas
                    .Where(e => e.EntregadorId == entregadorId)
                    .Include(e => e.Entregador)
                    .ToListAsync();

                // Obtém as entregas do MongoDB
                var filter = Builders<Entrega>.Filter.Eq("EntregadorId", entregadorId);
                var entregasMongo = await _mongoCollection.Find(filter).ToListAsync();

                // Retorna a união de ambos
                var result = entregasSql.Union(entregasMongo).ToList();

                _logger.LogInformation("Successfully fetched {Count} entregas for Entregador ID: {EntregadorId}.", result.Count, entregadorId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching entregas for Entregador ID: {EntregadorId}.", entregadorId);
                throw;
            }
        }

        public async Task UpdateEntregaAsync(Entrega entrega)
        {
            try
            {
                _logger.LogInformation("Updating entrega with ID: {Id}", entrega.Id);

                // Atualiza no PostgreSQL
                _context.Entregas.Update(entrega);
                await _context.SaveChangesAsync();

                // Atualiza no MongoDB
                var filter = Builders<Entrega>.Filter.Eq("Id", entrega.Id);
                await _mongoCollection.ReplaceOneAsync(filter, entrega);

                _logger.LogInformation("Entrega successfully updated with ID: {Id}", entrega.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating entrega with ID: {Id}", entrega.Id);
                throw;
            }
        }

        public async Task RemoveEntregaAsync(Entrega entrega)
        {
            try
            {
                _logger.LogInformation("Removing entrega with ID: {Id}", entrega.Id);

                // Remove do PostgreSQL
                _context.Entregas.Remove(entrega);
                await _context.SaveChangesAsync();

                // Remove do MongoDB
                var filter = Builders<Entrega>.Filter.Eq("Id", entrega.Id);
                await _mongoCollection.DeleteOneAsync(filter);

                _logger.LogInformation("Entrega successfully removed with ID: {Id}", entrega.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while removing entrega with ID: {Id}", entrega.Id);
                throw;
            }
        }
    }
}
