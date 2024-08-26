// API/Controllers/MotoController.cs
using DesafioRentDelivery.Application.DTOs;
using DesafioRentDelivery.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesafioRentDelivery.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MotoController : ControllerBase
    {
        private readonly IMotoService _motoService;

        public MotoController(IMotoService motoService)
        {
            _motoService = motoService;
        }

        /// <summary>
        /// Retorna uma lista de todas as motos.
        /// </summary>
        /// <returns>Um IEnumerable<MotoDTO> com os detalhes de cada moto.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MotoDTO>>> GetAllMotos()
        {
            try
            {
                Log.Information("Fetching all motos.");
                var motos = await _motoService.GetAllMotosAsync();
                return Ok(motos);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while fetching all motos.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Retorna os detalhes de uma moto específica com base no ID fornecido.
        /// </summary>
        /// <param name="id">O ID da moto que se deseja buscar.</param>
        /// <returns>Um MotoDTO com os detalhes da moto.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<MotoDTO>> GetMotoById(int id)
        {
            try
            {
                Log.Information("Fetching moto with ID: {Id}", id);
                var moto = await _motoService.GetMotoByIdAsync(id);
                if (moto == null)
                {
                    Log.Warning("Moto with ID: {Id} not found.", id);
                    return NotFound();
                }
                return Ok(moto);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while fetching moto with ID: {Id}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Cria uma nova moto com os dados fornecidos.
        /// </summary>
        /// <param name="motoDto">MotoDTO no corpo da solicitação.</param>
        /// <returns>A moto recém-criada, com seu ID atribuído.</returns>
        [HttpPost]
        public async Task<ActionResult> CreateMoto([FromBody] MotoDTO motoDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Log.Warning("Invalid model state for moto creation.");
                    return BadRequest(ModelState);
                }

                Log.Information("Creating a new moto.");
                await _motoService.AddMotoAsync(motoDto);
                return CreatedAtAction(nameof(GetMotoById), new { id = motoDto.Id }, motoDto);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while creating a new moto.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Atualiza os detalhes de uma moto existente.
        /// </summary>
        /// <param name="id">O ID da moto a ser atualizada.</param>
        /// <param name="motoDto">MotoDTO no corpo da solicitação.</param>
        /// <returns>204 Se a operação for bem-sucedida.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMoto(int id, [FromBody] MotoDTO motoDto)
        {
            try
            {
                if (id != motoDto.Id)
                {
                    Log.Warning("Moto ID mismatch. ID from route: {RouteId}, ID from body: {BodyId}", id, motoDto.Id);
                    return BadRequest("Moto ID mismatch");
                }

                Log.Information("Updating moto with ID: {Id}", id);
                var existingMoto = await _motoService.GetMotoByIdAsync(id);
                if (existingMoto == null)
                {
                    Log.Warning("Moto with ID: {Id} not found for update.", id);
                    return NotFound();
                }

                await _motoService.UpdateMotoAsync(motoDto);
                return NoContent();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while updating moto with ID: {Id}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Remove uma moto com base no ID fornecido.
        /// </summary>
        /// <param name="id">O ID da moto a ser removida.</param>
        /// <returns>204 Se a operação for bem-sucedida.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMoto(int id)
        {
            try
            {
                Log.Information("Deleting moto with ID: {Id}", id);
                var existingMoto = await _motoService.GetMotoByIdAsync(id);
                if (existingMoto == null)
                {
                    Log.Warning("Moto with ID: {Id} not found for deletion.", id);
                    return NotFound();
                }

                await _motoService.RemoveMotoAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while deleting moto with ID: {Id}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
