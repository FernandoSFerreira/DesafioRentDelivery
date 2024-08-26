// Application/Services/EntregadorService.cs
using DesafioRentDelivery.Application.DTOs;
using DesafioRentDelivery.Domain.Entities;
using DesafioRentDelivery.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DesafioRentDelivery.Application.Services
{
    public class EntregadorService : IEntregadorService
    {
        private readonly IEntregadorRepository _entregadorRepository;
        private readonly IAluguelRepository _aluguelRepository;
        private readonly IEntregaRepository _entregaRepository;
        private readonly ILogger<EntregadorService> _logger;

        public EntregadorService(
            IEntregadorRepository entregadorRepository,
            IAluguelRepository aluguelRepository,
            IEntregaRepository entregaRepository,
            ILogger<EntregadorService> logger) // Injeção do logger
        {
            _entregadorRepository = entregadorRepository;
            _aluguelRepository = aluguelRepository;
            _entregaRepository = entregaRepository;
            _logger = logger;
        }

        // Adiciona um novo entregador
        public async Task AddEntregadorAsync(EntregadorDTO entregadorDto)
        {
            try
            {
                _logger.LogInformation("Adding a new entregador with Documento: {Documento}", entregadorDto.Documento);

                var entregador = new Entregador
                {
                    Nome = entregadorDto.Nome,
                    Documento = entregadorDto.Documento,
                    Telefone = entregadorDto.Telefone
                };

                await _entregadorRepository.AddEntregadorAsync(entregador);
                _logger.LogInformation("Successfully added entregador with ID: {Id}", entregador.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a new entregador with Documento: {Documento}", entregadorDto.Documento);
                throw;
            }
        }

        // Obtém um entregador por ID, incluindo aluguéis e entregas associados
        public async Task<EntregadorDTO> GetEntregadorByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Fetching entregador with ID: {Id}", id);

                var entregador = await _entregadorRepository.GetEntregadorByIdAsync(id);
                if (entregador == null)
                {
                    _logger.LogWarning("Entregador with ID: {Id} not found.", id);
                    return null;
                }

                var alugueis = await _aluguelRepository.GetAlugueisByEntregadorIdAsync(id);
                var entregas = await _entregaRepository.GetEntregasByEntregadorIdAsync(id);

                return new EntregadorDTO
                {
                    Id = entregador.Id,
                    Nome = entregador.Nome,
                    Documento = entregador.Documento,
                    Telefone = entregador.Telefone,
                    Alugueis = alugueis.Select(a => new AluguelDTO
                    {
                        Id = a.Id,
                        MotoId = a.MotoId,
                        DataInicio = a.DataInicio,
                        DataFim = a.DataFim
                    }).ToList(),
                    Entregas = entregas.Select(e => new EntregaDTO
                    {
                        Id = e.Id,
                        EntregadorId = e.EntregadorId,
                        DataEntrega = e.DataEntrega,
                        Destino = e.Destino,
                        Status = e.Status
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching entregador with ID: {Id}", id);
                throw;
            }
        }

        // Obtém todos os entregadores
        public async Task<IEnumerable<EntregadorDTO>> GetAllEntregadoresAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all entregadores.");

                var entregadores = await _entregadorRepository.GetAllEntregadoresAsync();
                var entregadorDtos = new List<EntregadorDTO>();

                foreach (var entregador in entregadores)
                {
                    var alugueis = await _aluguelRepository.GetAlugueisByEntregadorIdAsync(entregador.Id);
                    var entregas = await _entregaRepository.GetEntregasByEntregadorIdAsync(entregador.Id);

                    var entregadorDto = new EntregadorDTO
                    {
                        Id = entregador.Id,
                        Nome = entregador.Nome,
                        Documento = entregador.Documento,
                        Telefone = entregador.Telefone,
                        Alugueis = alugueis.Select(a => new AluguelDTO
                        {
                            Id = a.Id,
                            MotoId = a.MotoId,
                            DataInicio = a.DataInicio,
                            DataFim = a.DataFim
                        }).ToList(),
                        Entregas = entregas.Select(e => new EntregaDTO
                        {
                            Id = e.Id,
                            EntregadorId = e.EntregadorId,
                            DataEntrega = e.DataEntrega,
                            Destino = e.Destino,
                            Status = e.Status
                        }).ToList()
                    };

                    entregadorDtos.Add(entregadorDto);
                }

                return entregadorDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all entregadores.");
                throw;
            }
        }

        // Atualiza um entregador existente
        public async Task UpdateEntregadorAsync(EntregadorDTO entregadorDto)
        {
            try
            {
                _logger.LogInformation("Updating entregador with ID: {Id}", entregadorDto.Id);

                var entregador = await _entregadorRepository.GetEntregadorByIdAsync(entregadorDto.Id);
                if (entregador == null)
                {
                    _logger.LogWarning("Entregador with ID: {Id} not found.", entregadorDto.Id);
                    throw new KeyNotFoundException("Entregador não encontrado.");
                }

                entregador.Nome = entregadorDto.Nome;
                entregador.Documento = entregadorDto.Documento;
                entregador.Telefone = entregadorDto.Telefone;

                await _entregadorRepository.UpdateEntregadorAsync(entregador);
                _logger.LogInformation("Successfully updated entregador with ID: {Id}", entregador.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating entregador with ID: {Id}", entregadorDto.Id);
                throw;
            }
        }

        // Remove um entregador por ID
        public async Task RemoveEntregadorAsync(int id)
        {
            try
            {
                _logger.LogInformation("Removing entregador with ID: {Id}", id);

                var entregador = await _entregadorRepository.GetEntregadorByIdAsync(id);
                if (entregador == null)
                {
                    _logger.LogWarning("Entregador with ID: {Id} not found.", id);
                    throw new KeyNotFoundException("Entregador não encontrado.");
                }

                await _entregadorRepository.RemoveEntregadorAsync(entregador);
                _logger.LogInformation("Successfully removed entregador with ID: {Id}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while removing entregador with ID: {Id}", id);
                throw;
            }
        }
    }
}
