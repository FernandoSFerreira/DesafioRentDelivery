// Infrastructure/Repositories/AluguelRepository.cs
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
    public class AluguelRepository : IAluguelRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMongoCollection<Aluguel> _mongoCollection;
        private readonly ILogger<AluguelRepository> _logger;

        public AluguelRepository(ApplicationDbContext context, IMongoClient mongoClient, ILogger<AluguelRepository> logger)
        {
            _context = context;
            _logger = logger;

            var database = mongoClient.GetDatabase("RentDeliveryDb");
            _mongoCollection = database.GetCollection<Aluguel>("Alugueis");
        }

        public async Task AddAluguelAsync(Aluguel aluguel)
        {
            try
            {
                _logger.LogInformation("Adding a new aluguel with Entregador ID: {EntregadorId} and Moto ID: {MotoId}", aluguel.EntregadorId, aluguel.MotoId);

                // Adiciona no PostgreSQL
                await _context.Alugueis.AddAsync(aluguel);
                await _context.SaveChangesAsync();

                // Adiciona no MongoDB
                await _mongoCollection.InsertOneAsync(aluguel);

                _logger.LogInformation("Aluguel successfully added with ID: {Id}", aluguel.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a new aluguel.");
                throw;
            }
        }

        public async Task<Aluguel> GetAluguelByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Fetching aluguel with ID: {Id}", id);

                // Tenta obter o aluguel do PostgreSQL
                var aluguel = await _context.Alugueis
                    .Include(a => a.Entregador)
                    .Include(a => a.Moto)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (aluguel == null)
                {
                    _logger.LogWarning("Aluguel with ID: {Id} not found in PostgreSQL, checking MongoDB.", id);

                    // Se não encontrar no PostgreSQL, tenta obter do MongoDB
                    var filter = Builders<Aluguel>.Filter.Eq("Id", id);
                    aluguel = await _mongoCollection.Find(filter).FirstOrDefaultAsync();

                    if (aluguel == null)
                    {
                        _logger.LogWarning("Aluguel with ID: {Id} not found in MongoDB.", id);
                    }
                }

                return aluguel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching aluguel with ID: {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<Aluguel>> GetAllAlugueisAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all alugueis from PostgreSQL and MongoDB.");

                // Obtém todos os aluguéis do PostgreSQL
                var alugueisSql = await _context.Alugueis
                    .Include(a => a.Entregador)
                    .Include(a => a.Moto)
                    .ToListAsync();

                // Obtém todos os aluguéis do MongoDB
                var alugueisMongo = await _mongoCollection.Find(_ => true).ToListAsync();

                // Retorna a união de ambos
                var result = alugueisSql.Union(alugueisMongo).ToList();

                _logger.LogInformation("Successfully fetched {Count} alugueis.", result.Count);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all alugueis.");
                throw;
            }
        }

        public async Task<IEnumerable<Aluguel>> GetAlugueisByEntregadorIdAsync(int entregadorId)
        {
            try
            {
                _logger.LogInformation("Fetching alugueis for Entregador ID: {EntregadorId} from PostgreSQL and MongoDB.", entregadorId);

                // Obtém os aluguéis do PostgreSQL
                var alugueisSql = await _context.Alugueis
                    .Where(a => a.EntregadorId == entregadorId)
                    .Include(a => a.Entregador)
                    .Include(a => a.Moto)
                    .ToListAsync();

                // Obtém os aluguéis do MongoDB
                var filter = Builders<Aluguel>.Filter.Eq("EntregadorId", entregadorId);
                var alugueisMongo = await _mongoCollection.Find(filter).ToListAsync();

                // Retorna a união de ambos
                var result = alugueisSql.Union(alugueisMongo).ToList();

                _logger.LogInformation("Successfully fetched {Count} alugueis for Entregador ID: {EntregadorId}.", result.Count, entregadorId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching alugueis for Entregador ID: {EntregadorId}.", entregadorId);
                throw;
            }
        }

        public async Task<Aluguel> GetAluguelAtivoByEntregadorIdAsync(int entregadorId)
        {
            try
            {
                _logger.LogInformation("Fetching active aluguel for Entregador ID: {EntregadorId}", entregadorId);

                // Tenta obter o aluguel ativo do PostgreSQL
                var aluguel = await _context.Alugueis
                    .Include(a => a.Entregador)
                    .Include(a => a.Moto)
                    .FirstOrDefaultAsync(a => a.EntregadorId == entregadorId && a.DataInicio <= DateTime.Now && a.DataFim >= DateTime.Now);

                if (aluguel == null)
                {
                    _logger.LogWarning("Active aluguel for Entregador ID: {EntregadorId} not found in PostgreSQL, checking MongoDB.", entregadorId);

                    // Se não encontrar no PostgreSQL, tenta obter do MongoDB
                    var filter = Builders<Aluguel>.Filter.Eq("EntregadorId", entregadorId) &
                                 Builders<Aluguel>.Filter.Lte("DataInicio", DateTime.Now) &
                                 Builders<Aluguel>.Filter.Gte("DataFim", DateTime.Now);
                    aluguel = await _mongoCollection.Find(filter).FirstOrDefaultAsync();

                    if (aluguel == null)
                    {
                        _logger.LogWarning("Active aluguel for Entregador ID: {EntregadorId} not found in MongoDB.", entregadorId);
                    }
                }

                return aluguel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching active aluguel for Entregador ID: {EntregadorId}.", entregadorId);
                throw;
            }
        }

        public async Task UpdateAluguelAsync(Aluguel aluguel)
        {
            try
            {
                _logger.LogInformation("Updating aluguel with ID: {Id}", aluguel.Id);

                // Atualiza no PostgreSQL
                _context.Alugueis.Update(aluguel);
                await _context.SaveChangesAsync();

                // Atualiza no MongoDB
                var filter = Builders<Aluguel>.Filter.Eq("Id", aluguel.Id);
                await _mongoCollection.ReplaceOneAsync(filter, aluguel);

                _logger.LogInformation("Aluguel successfully updated with ID: {Id}", aluguel.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating aluguel with ID: {Id}", aluguel.Id);
                throw;
            }
        }

        public async Task RemoveAluguelAsync(Aluguel aluguel)
        {
            try
            {
                _logger.LogInformation("Removing aluguel with ID: {Id}", aluguel.Id);

                // Remove do PostgreSQL
                _context.Alugueis.Remove(aluguel);
                await _context.SaveChangesAsync();

                // Remove do MongoDB
                var filter = Builders<Aluguel>.Filter.Eq("Id", aluguel.Id);
                await _mongoCollection.DeleteOneAsync(filter);

                _logger.LogInformation("Aluguel successfully removed with ID: {Id}", aluguel.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while removing aluguel with ID: {Id}", aluguel.Id);
                throw;
            }
        }
    }
}
