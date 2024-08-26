// Application/Services/EntregaService.cs
using DesafioRentDelivery.Application.DTOs;
using DesafioRentDelivery.Domain.Entities;
using DesafioRentDelivery.Domain.Repositories;
using DesafioRentDelivery.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DesafioRentDelivery.Application.Services
{
    public class EntregaService : IEntregaService
    {
        private readonly IEntregaRepository _entregaRepository;
        private readonly IAluguelRepository _aluguelRepository;
        private readonly IRabbitMqService _rabbitMqService;
        private readonly ILogger<EntregaService> _logger;

        public EntregaService(
            IEntregaRepository entregaRepository,
            IAluguelRepository aluguelRepository,
            IRabbitMqService rabbitMqService,
            ILogger<EntregaService> logger) // Injeção do logger
        {
            _entregaRepository = entregaRepository;
            _aluguelRepository = aluguelRepository;
            _rabbitMqService = rabbitMqService;
            _logger = logger;
        }

        // Adiciona uma nova entrega, verificando se o entregador tem um aluguel ativo
        public async Task AddEntregaAsync(EntregaDTO entregaDto)
        {
            try
            {
                _logger.LogInformation("Starting to add a new entrega for Entregador ID: {EntregadorId}", entregaDto.EntregadorId);

                // Verifica se o entregador tem um aluguel ativo
                var aluguelAtivo = await _aluguelRepository.GetAluguelAtivoByEntregadorIdAsync(entregaDto.EntregadorId);
                if (aluguelAtivo == null)
                {
                    _logger.LogWarning("Entregador with ID: {EntregadorId} does not have an active aluguel.", entregaDto.EntregadorId);
                    throw new Exception("Entregador não tem um aluguel de moto ativo.");
                }

                // Cria uma nova entidade Entrega
                var entrega = new Entrega
                {
                    EntregadorId = entregaDto.EntregadorId,
                    DataEntrega = entregaDto.DataEntrega,
                    Destino = entregaDto.Destino,
                    Status = "Pendente"
                };

                // Salva a entrega no banco de dados
                await _entregaRepository.AddEntregaAsync(entrega);
                _logger.LogInformation("Entrega successfully added with ID: {Id}", entrega.Id);

                // Envia uma mensagem ao RabbitMQ
                _rabbitMqService.SendMessage($"Nova entrega criada: {entrega.Id}, Entregador: {entrega.EntregadorId}");
                _logger.LogInformation("Message sent to RabbitMQ for Entrega ID: {Id}", entrega.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a new entrega for Entregador ID: {EntregadorId}", entregaDto.EntregadorId);
                throw;
            }
        }

        // Obtém uma entrega por ID
        public async Task<EntregaDTO> GetEntregaByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Fetching entrega with ID: {Id}", id);

                var entrega = await _entregaRepository.GetEntregaByIdAsync(id);
                if (entrega == null)
                {
                    _logger.LogWarning("Entrega with ID: {Id} not found.", id);
                    return null;
                }

                return new EntregaDTO
                {
                    Id = entrega.Id,
                    EntregadorId = entrega.EntregadorId,
                    DataEntrega = entrega.DataEntrega,
                    Destino = entrega.Destino,
                    Status = entrega.Status
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching entrega with ID: {Id}", id);
                throw;
            }
        }

        // Obtém todas as entregas
        public async Task<IEnumerable<EntregaDTO>> GetAllEntregasAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all entregas.");

                var entregas = await _entregaRepository.GetAllEntregasAsync();
                return entregas.Select(e => new EntregaDTO
                {
                    Id = e.Id,
                    EntregadorId = e.EntregadorId,
                    DataEntrega = e.DataEntrega,
                    Destino = e.Destino,
                    Status = e.Status
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all entregas.");
                throw;
            }
        }

        // Atualiza uma entrega existente
        public async Task UpdateEntregaAsync(EntregaDTO entregaDto)
        {
            try
            {
                _logger.LogInformation("Updating entrega with ID: {Id}", entregaDto.Id);

                var entrega = await _entregaRepository.GetEntregaByIdAsync(entregaDto.Id);
                if (entrega == null)
                {
                    _logger.LogWarning("Entrega with ID: {Id} not found.", entregaDto.Id);
                    throw new Exception("Entrega não encontrada.");
                }

                entrega.Destino = entregaDto.Destino;
                entrega.DataEntrega = entregaDto.DataEntrega;
                entrega.Status = entregaDto.Status;

                await _entregaRepository.UpdateEntregaAsync(entrega);
                _logger.LogInformation("Entrega successfully updated with ID: {Id}", entrega.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating entrega with ID: {Id}", entregaDto.Id);
                throw;
            }
        }

        // Remove uma entrega por ID
        public async Task RemoveEntregaAsync(int id)
        {
            try
            {
                _logger.LogInformation("Removing entrega with ID: {Id}", id);

                var entrega = await _entregaRepository.GetEntregaByIdAsync(id);
                if (entrega == null)
                {
                    _logger.LogWarning("Entrega with ID: {Id} not found.", id);
                    throw new Exception("Entrega não encontrada.");
                }

                await _entregaRepository.RemoveEntregaAsync(entrega);
                _logger.LogInformation("Entrega successfully removed with ID: {Id}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while removing entrega with ID: {Id}", id);
                throw;
            }
        }
    }
}
