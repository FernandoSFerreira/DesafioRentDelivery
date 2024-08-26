// API/Controllers/EntregaController.cs
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
    public class EntregaController : ControllerBase
    {
        private readonly IEntregaService _entregaService;

        public EntregaController(IEntregaService entregaService)
        {
            _entregaService = entregaService;
        }

        // GET: api/entrega
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EntregaDTO>>> GetAllEntregas()
        {
            try
            {
                Log.Information("Fetching all entregas.");
                var entregas = await _entregaService.GetAllEntregasAsync();
                return Ok(entregas);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while fetching all entregas.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: api/entrega/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<EntregaDTO>> GetEntregaById(int id)
        {
            try
            {
                Log.Information("Fetching entrega with ID: {Id}", id);
                var entrega = await _entregaService.GetEntregaByIdAsync(id);

                if (entrega == null)
                {
                    Log.Warning("Entrega with ID: {Id} not found.", id);
                    return NotFound();
                }

                return Ok(entrega);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while fetching entrega with ID: {Id}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // POST: api/entrega
        [HttpPost]
        public async Task<ActionResult> CreateEntrega([FromBody] EntregaDTO entregaDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Log.Warning("Invalid model state for entrega creation.");
                    return BadRequest(ModelState);
                }

                Log.Information("Creating a new entrega.");
                await _entregaService.AddEntregaAsync(entregaDto);
                return CreatedAtAction(nameof(GetEntregaById), new { id = entregaDto.Id }, entregaDto);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while creating a new entrega.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // PUT: api/entrega/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEntrega(int id, [FromBody] EntregaDTO entregaDto)
        {
            try
            {
                if (id != entregaDto.Id)
                {
                    Log.Warning("Entrega ID mismatch. ID from route: {RouteId}, ID from body: {BodyId}", id, entregaDto.Id);
                    return BadRequest("Entrega ID mismatch");
                }

                Log.Information("Updating entrega with ID: {Id}", id);
                var existingEntrega = await _entregaService.GetEntregaByIdAsync(id);
                if (existingEntrega == null)
                {
                    Log.Warning("Entrega with ID: {Id} not found for update.", id);
                    return NotFound();
                }

                await _entregaService.UpdateEntregaAsync(entregaDto);
                return NoContent();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while updating entrega with ID: {Id}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // DELETE: api/entrega/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEntrega(int id)
        {
            try
            {
                Log.Information("Deleting entrega with ID: {Id}", id);
                var existingEntrega = await _entregaService.GetEntregaByIdAsync(id);
                if (existingEntrega == null)
                {
                    Log.Warning("Entrega with ID: {Id} not found for deletion.", id);
                    return NotFound();
                }

                await _entregaService.RemoveEntregaAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while deleting entrega with ID: {Id}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
