// Application/Services/AluguelService.cs
using DesafioRentDelivery.Application.DTOs;
using DesafioRentDelivery.Domain.Entities;
using DesafioRentDelivery.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DesafioRentDelivery.Application.Services
{
    public class AluguelService : IAluguelService
    {
        private readonly IAluguelRepository _aluguelRepository;
        private readonly IEntregadorRepository _entregadorRepository;
        private readonly IMotoRepository _motoRepository;
        private readonly ILogger<AluguelService> _logger;

        public AluguelService(
            IAluguelRepository aluguelRepository,
            IEntregadorRepository entregadorRepository,
            IMotoRepository motoRepository,
            ILogger<AluguelService> logger)
        {
            _aluguelRepository = aluguelRepository;
            _entregadorRepository = entregadorRepository;
            _motoRepository = motoRepository;
            _logger = logger;
        }

        public async Task AddAluguelAsync(AluguelDTO aluguelDto)
        {
            try
            {
                Log.Information("Starting to add a new aluguel for Entregador ID: {EntregadorId} and Moto ID: {MotoId}.", aluguelDto.EntregadorId, aluguelDto.MotoId);

                // Validação adicional (verificando se a moto e o entregador existem)
                var entregador = await _entregadorRepository.GetEntregadorByIdAsync(aluguelDto.EntregadorId);
                if (entregador == null)
                {
                    Log.Warning("Entregador not found with ID: {EntregadorId}", aluguelDto.EntregadorId);
                    throw new KeyNotFoundException("Entregador não encontrado.");
                }

                var moto = await _motoRepository.GetMotoByIdAsync(aluguelDto.MotoId);
                if (moto == null)
                {
                    Log.Warning("Moto not found with ID: {MotoId}", aluguelDto.MotoId);
                    throw new KeyNotFoundException("Moto não encontrada.");
                }

                var aluguel = new Aluguel
                {
                    EntregadorId = aluguelDto.EntregadorId,
                    MotoId = aluguelDto.MotoId,
                    DataInicio = aluguelDto.DataInicio,
                    DataFim = aluguelDto.DataFim
                };

                await _aluguelRepository.AddAluguelAsync(aluguel);
                Log.Information("Aluguel successfully added with ID: {Id}", aluguel.Id);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while adding a new aluguel for Entregador ID: {EntregadorId} and Moto ID: {MotoId}.", aluguelDto.EntregadorId, aluguelDto.MotoId);
                throw;
            }
        }

        public async Task<AluguelDTO> GetAluguelByIdAsync(int id)
        {
            try
            {
                Log.Information("Fetching aluguel with ID: {Id}", id);
                var aluguel = await _aluguelRepository.GetAluguelByIdAsync(id);
                if (aluguel == null)
                {
                    Log.Warning("Aluguel not found with ID: {Id}", id);
                    return null;
                }

                return new AluguelDTO
                {
                    Id = aluguel.Id,
                    EntregadorId = aluguel.EntregadorId,
                    MotoId = aluguel.MotoId,
                    DataInicio = aluguel.DataInicio,
                    DataFim = aluguel.DataFim
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while fetching aluguel with ID: {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<AluguelDTO>> GetAllAlugueisAsync()
        {
            try
            {
                Log.Information("Fetching all aluguels.");
                var alugueis = await _aluguelRepository.GetAllAlugueisAsync();
                return alugueis.Select(aluguel => new AluguelDTO
                {
                    Id = aluguel.Id,
                    EntregadorId = aluguel.EntregadorId,
                    MotoId = aluguel.MotoId,
                    DataInicio = aluguel.DataInicio,
                    DataFim = aluguel.DataFim
                }).ToList();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while fetching all aluguels.");
                throw;
            }
        }

        public async Task<IEnumerable<AluguelDTO>> GetAlugueisByEntregadorIdAsync(int entregadorId)
        {
            try
            {
                Log.Information("Fetching aluguels for Entregador ID: {EntregadorId}", entregadorId);
                var alugueis = await _aluguelRepository.GetAlugueisByEntregadorIdAsync(entregadorId);
                return alugueis.Select(aluguel => new AluguelDTO
                {
                    Id = aluguel.Id,
                    EntregadorId = aluguel.EntregadorId,
                    MotoId = aluguel.MotoId,
                    DataInicio = aluguel.DataInicio,
                    DataFim = aluguel.DataFim
                }).ToList();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while fetching aluguels for Entregador ID: {EntregadorId}", entregadorId);
                throw;
            }
        }

        public async Task<AluguelDTO> GetAluguelAtivoByEntregadorIdAsync(int entregadorId)
        {
            try
            {
                Log.Information("Fetching active aluguel for Entregador ID: {EntregadorId}", entregadorId);
                var aluguel = await _aluguelRepository.GetAluguelAtivoByEntregadorIdAsync(entregadorId);
                if (aluguel == null)
                {
                    Log.Warning("No active aluguel found for Entregador ID: {EntregadorId}", entregadorId);
                    return null;
                }

                return new AluguelDTO
                {
                    Id = aluguel.Id,
                    EntregadorId = aluguel.EntregadorId,
                    MotoId = aluguel.MotoId,
                    DataInicio = aluguel.DataInicio,
                    DataFim = aluguel.DataFim
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while fetching active aluguel for Entregador ID: {EntregadorId}", entregadorId);
                throw;
            }
        }

        public async Task UpdateAluguelAsync(AluguelDTO aluguelDto)
        {
            try
            {
                Log.Information("Updating aluguel with ID: {Id}", aluguelDto.Id);
                var aluguel = await _aluguelRepository.GetAluguelByIdAsync(aluguelDto.Id);
                if (aluguel == null)
                {
                    Log.Warning("Aluguel not found with ID: {Id}", aluguelDto.Id);
                    throw new KeyNotFoundException("Aluguel não encontrado.");
                }

                aluguel.EntregadorId = aluguelDto.EntregadorId;
                aluguel.MotoId = aluguelDto.MotoId;
                aluguel.DataInicio = aluguelDto.DataInicio;
                aluguel.DataFim = aluguelDto.DataFim;

                await _aluguelRepository.UpdateAluguelAsync(aluguel);
                Log.Information("Aluguel successfully updated with ID: {Id}", aluguel.Id);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while updating aluguel with ID: {Id}", aluguelDto.Id);
                throw;
            }
        }

        public async Task RemoveAluguelAsync(int id)
        {
            try
            {
                Log.Information("Removing aluguel with ID: {Id}", id);
                var aluguel = await _aluguelRepository.GetAluguelByIdAsync(id);
                if (aluguel == null)
                {
                    Log.Warning("Aluguel not found with ID: {Id}", id);
                    throw new KeyNotFoundException("Aluguel não encontrado.");
                }

                await _aluguelRepository.RemoveAluguelAsync(aluguel);
                Log.Information("Aluguel successfully removed with ID: {Id}", id);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while removing aluguel with ID: {Id}", id);
                throw;
            }
        }
    }
}
