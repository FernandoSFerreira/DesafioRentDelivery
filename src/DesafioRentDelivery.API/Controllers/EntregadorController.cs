// API/Controllers/EntregadorController.cs
using DesafioRentDelivery.Application.DTOs;
using DesafioRentDelivery.Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesafioRentDelivery.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EntregadorController : ControllerBase
    {
        private readonly IEntregadorService _entregadorService;

        public EntregadorController(IEntregadorService entregadorService)
        {
            _entregadorService = entregadorService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EntregadorDTO>>> GetAllEntregadores()
        {
            var entregadores = await _entregadorService.GetAllEntregadoresAsync();
            return Ok(entregadores);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EntregadorDTO>> GetEntregadorById(int id)
        {
            var entregador = await _entregadorService.GetEntregadorByIdAsync(id);
            if (entregador == null)
            {
                return NotFound();
            }
            return Ok(entregador);
        }

        [HttpPost]
        public async Task<ActionResult> CreateEntregador([FromBody] EntregadorDTO entregadorDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _entregadorService.AddEntregadorAsync(entregadorDto);
            return CreatedAtAction(nameof(GetEntregadorById), new { id = entregadorDto.Id }, entregadorDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEntregador(int id, [FromBody] EntregadorDTO entregadorDto)
        {
            if (id != entregadorDto.Id)
            {
                return BadRequest("Entregador ID mismatch");
            }

            var existingEntregador = await _entregadorService.GetEntregadorByIdAsync(id);
            if (existingEntregador == null)
            {
                return NotFound();
            }

            await _entregadorService.UpdateEntregadorAsync(entregadorDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEntregador(int id)
        {
            var existingEntregador = await _entregadorService.GetEntregadorByIdAsync(id);
            if (existingEntregador == null)
            {
                return NotFound();
            }

            await _entregadorService.RemoveEntregadorAsync(id);
            return NoContent();
        }
    }
}