// API/Controllers/AluguelController.cs
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
    public class AluguelController : ControllerBase
    {
        private readonly IAluguelService _aluguelService;

        public AluguelController(IAluguelService aluguelService)
        {
            _aluguelService = aluguelService;
        }

        // GET: api/aluguel
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AluguelDTO>>> GetAllAlugueis()
        {
            try
            {
                Log.Information("Fetching all aluguels.");
                var alugueis = await _aluguelService.GetAllAlugueisAsync();
                return Ok(alugueis);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while fetching all aluguels.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: api/aluguel/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<AluguelDTO>> GetAluguelById(int id)
        {
            try
            {
                Log.Information("Fetching aluguel with ID: {Id}", id);
                var aluguel = await _aluguelService.GetAluguelByIdAsync(id);

                if (aluguel == null)
                {
                    Log.Warning("Aluguel with ID: {Id} not found.", id);
                    return NotFound();
                }

                return Ok(aluguel);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while fetching aluguel with ID: {Id}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // POST: api/aluguel
        [HttpPost]
        public async Task<ActionResult> CreateAluguel([FromBody] AluguelDTO aluguelDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Log.Warning("Invalid model state for aluguel creation.");
                    return BadRequest(ModelState);
                }

                Log.Information("Creating a new aluguel.");
                await _aluguelService.AddAluguelAsync(aluguelDto);
                return CreatedAtAction(nameof(GetAluguelById), new { id = aluguelDto.Id }, aluguelDto);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while creating a new aluguel.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // PUT: api/aluguel/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAluguel(int id, [FromBody] AluguelDTO aluguelDto)
        {
            try
            {
                if (id != aluguelDto.Id)
                {
                    Log.Warning("Aluguel ID mismatch. ID from route: {RouteId}, ID from body: {BodyId}", id, aluguelDto.Id);
                    return BadRequest("Aluguel ID mismatch");
                }

                Log.Information("Updating aluguel with ID: {Id}", id);
                var existingAluguel = await _aluguelService.GetAluguelByIdAsync(id);
                if (existingAluguel == null)
                {
                    Log.Warning("Aluguel with ID: {Id} not found for update.", id);
                    return NotFound();
                }

                await _aluguelService.UpdateAluguelAsync(aluguelDto);
                return NoContent();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while updating aluguel with ID: {Id}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // DELETE: api/aluguel/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAluguel(int id)
        {
            try
            {
                Log.Information("Deleting aluguel with ID: {Id}", id);
                var existingAluguel = await _aluguelService.GetAluguelByIdAsync(id);
                if (existingAluguel == null)
                {
                    Log.Warning("Aluguel with ID: {Id} not found for deletion.", id);
                    return NotFound();
                }

                await _aluguelService.RemoveAluguelAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while deleting aluguel with ID: {Id}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
