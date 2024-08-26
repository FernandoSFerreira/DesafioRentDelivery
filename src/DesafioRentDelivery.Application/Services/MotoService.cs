// Application/Services/MotoService.cs
using DesafioRentDelivery.Application.DTOs;
using DesafioRentDelivery.Domain.Entities;
using DesafioRentDelivery.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesafioRentDelivery.Application.Services
{
    public class MotoService : IMotoService
    {
        private readonly IMotoRepository _motoRepository;
        private readonly ILogger<MotoService> _logger;

        public MotoService(IMotoRepository motoRepository, ILogger<MotoService> logger)
        {
            _motoRepository = motoRepository;
            _logger = logger;
        }

        public async Task AddMotoAsync(MotoDTO motoDto)
        {
            try
            {
                _logger.LogInformation("Adding a new moto with Placa: {Placa}", motoDto.Placa);

                var moto = new Moto
                {
                    Placa = motoDto.Placa,
                    Modelo = motoDto.Modelo,
                    Chassi = motoDto.Chassi
                };

                await _motoRepository.AddMotoAsync(moto);
                _logger.LogInformation("Moto successfully added with ID: {Id}", moto.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a new moto with Placa: {Placa}", motoDto.Placa);
                throw;
            }
        }

        public async Task<MotoDTO> GetMotoByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Fetching moto with ID: {Id}", id);

                var moto = await _motoRepository.GetMotoByIdAsync(id);
                if (moto == null)
                {
                    _logger.LogWarning("Moto with ID: {Id} not found.", id);
                    return null;
                }

                var historicoManutencoes = new List<HistoricoManutencaoDTO>();
                foreach (var historico in moto.HistoricoManutencoes)
                {
                    historicoManutencoes.Add(new HistoricoManutencaoDTO
                    {
                        Id = historico.Id,
                        DataManutencao = historico.DataManutencao,
                        Descricao = historico.Descricao
                    });
                }

                return new MotoDTO
                {
                    Id = moto.Id,
                    Placa = moto.Placa,
                    Modelo = moto.Modelo,
                    Chassi = moto.Chassi,
                    HistoricoManutencoes = historicoManutencoes
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching moto with ID: {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<MotoDTO>> GetAllMotosAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all motos.");

                var motos = await _motoRepository.GetAllMotosAsync();
                var motoDtos = new List<MotoDTO>();

                foreach (var moto in motos)
                {
                    var historicoManutencoes = new List<HistoricoManutencaoDTO>();
                    foreach (var historico in moto.HistoricoManutencoes)
                    {
                        historicoManutencoes.Add(new HistoricoManutencaoDTO
                        {
                            Id = historico.Id,
                            DataManutencao = historico.DataManutencao,
                            Descricao = historico.Descricao
                        });
                    }

                    motoDtos.Add(new MotoDTO
                    {
                        Id = moto.Id,
                        Placa = moto.Placa,
                        Modelo = moto.Modelo,
                        Chassi = moto.Chassi,
                        HistoricoManutencoes = historicoManutencoes
                    });
                }

                return motoDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all motos.");
                throw;
            }
        }

        public async Task UpdateMotoAsync(MotoDTO motoDto)
        {
            try
            {
                _logger.LogInformation("Updating moto with ID: {Id}", motoDto.Id);

                var moto = await _motoRepository.GetMotoByIdAsync(motoDto.Id);
                if (moto == null)
                {
                    _logger.LogWarning("Moto with ID: {Id} not found.", motoDto.Id);
                    return;
                }

                moto.Placa = motoDto.Placa;
                moto.Modelo = motoDto.Modelo;
                moto.Chassi = motoDto.Chassi;

                await _motoRepository.UpdateMotoAsync(moto);
                _logger.LogInformation("Moto successfully updated with ID: {Id}", moto.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating moto with ID: {Id}", motoDto.Id);
                throw;
            }
        }

        public async Task RemoveMotoAsync(int id)
        {
            try
            {
                _logger.LogInformation("Removing moto with ID: {Id}", id);

                var moto = await _motoRepository.GetMotoByIdAsync(id);
                if (moto == null)
                {
                    _logger.LogWarning("Moto with ID: {Id} not found.", id);
                    return;
                }

                await _motoRepository.RemoveMotoAsync(moto);
                _logger.LogInformation("Moto successfully removed with ID: {Id}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while removing moto with ID: {Id}", id);
                throw;
            }
        }

        public async Task AddHistoricoManutencaoAsync(int motoId, HistoricoManutencaoDTO historicoDto)
        {
            try
            {
                _logger.LogInformation("Adding historico de manutencao for Moto ID: {MotoId}", motoId);

                var historico = new HistoricoManutencao
                {
                    MotoId = motoId,
                    DataManutencao = historicoDto.DataManutencao,
                    Descricao = historicoDto.Descricao
                };

                await _motoRepository.AddHistoricoManutencaoAsync(historico);
                _logger.LogInformation("Historico de manutencao successfully added for Moto ID: {MotoId}", motoId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding historico de manutencao for Moto ID: {MotoId}", motoId);
                throw;
            }
        }

        public async Task RemoveHistoricoManutencaoAsync(int historicoId)
        {
            try
            {
                _logger.LogInformation("Removing historico de manutencao with ID: {HistoricoId}", historicoId);

                var historico = new HistoricoManutencao { Id = historicoId };
                await _motoRepository.RemoveHistoricoManutencaoAsync(historico);
                _logger.LogInformation("Historico de manutencao successfully removed with ID: {HistoricoId}", historicoId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while removing historico de manutencao with ID: {HistoricoId}", historicoId);
                throw;
            }
        }
    }
}
