// DesafioRentDelivery.IntegrationTests/Controllers/EntregaControllerTests.cs

using DesafioRentDelivery.Application.DTOs;
using DesafioRentDelivery.Infrastructure.Data;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace DesafioRentDelivery.IntegrationTests.Controllers
{
    public class EntregaControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly ApplicationDbContext _context;

        public EntregaControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            _context = factory.Services.GetService(typeof(ApplicationDbContext)) as ApplicationDbContext;
        }

        [Fact]
        public async Task GetAllEntregas_ShouldReturnOkResponseWithEntregas()
        {
            // Arrange
            var response = await _client.GetAsync("/api/entrega");

            // Act
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var entregas = await response.Content.ReadFromJsonAsync<IEnumerable<EntregaDTO>>();

            // Assert
            entregas.Should().NotBeNull();
            entregas.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetEntregaById_ShouldReturnOkResponseWithEntrega_WhenEntregaExists()
        {
            // Arrange
            var entrega = _context.Entregas.First();
            var response = await _client.GetAsync($"/api/entrega/{entrega.Id}");

            // Act
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<EntregaDTO>();

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(entrega.Id);
        }

        [Fact]
        public async Task GetEntregaById_ShouldReturnNotFound_WhenEntregaDoesNotExist()
        {
            // Arrange
            var invalidId = int.MaxValue;
            var response = await _client.GetAsync($"/api/entrega/{invalidId}");

            // Act
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CreateEntrega_ShouldReturnCreatedResponse_WhenEntregaIsValid()
        {
            // Arrange
            var entregador = _context.Entregadores.First();
            var aluguelAtivo = _context.Alugueis.First(a => a.EntregadorId == entregador.Id);
            var novaEntrega = new EntregaDTO
            {
                EntregadorId = entregador.Id,
                DataEntrega = DateTime.Now,
                Destino = "Rua A"
            };

            var response = await _client.PostAsJsonAsync("/api/entrega", novaEntrega);

            // Act
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var createdEntrega = await response.Content.ReadFromJsonAsync<EntregaDTO>();

            // Assert
            createdEntrega.Should().NotBeNull();
            createdEntrega.Id.Should().NotBe(0);
            createdEntrega.EntregadorId.Should().Be(novaEntrega.EntregadorId);
            createdEntrega.Destino.Should().Be(novaEntrega.Destino);
        }

        [Fact]
        public async Task CreateEntrega_ShouldReturnBadRequest_WhenEntregadorHasNoActiveAluguel()
        {
            // Arrange
            var entregador = _context.Entregadores.First();
            var novaEntrega = new EntregaDTO
            {
                EntregadorId = entregador.Id,
                DataEntrega = DateTime.Now,
                Destino = "Rua A"
            };

            // Remove o aluguel ativo para simular a ausência de aluguel ativo
            var aluguelAtivo = _context.Alugueis.First(a => a.EntregadorId == entregador.Id);
            _context.Alugueis.Remove(aluguelAtivo);
            await _context.SaveChangesAsync();

            var response = await _client.PostAsJsonAsync("/api/entrega", novaEntrega);

            // Act
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UpdateEntrega_ShouldReturnNoContentResponse_WhenEntregaIsValid()
        {
            // Arrange
            var entrega = _context.Entregas.First();
            var updatedEntrega = new EntregaDTO
            {
                Id = entrega.Id,
                EntregadorId = entrega.EntregadorId,
                DataEntrega = entrega.DataEntrega.AddDays(1),
                Destino = "Rua B",
                Status = "Concluída"
            };

            var response = await _client.PutAsJsonAsync($"/api/entrega/{entrega.Id}", updatedEntrega);

            // Act
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Verifying the update in the database
            var entregaInDb = await _context.Entregas.FindAsync(entrega.Id);
            entregaInDb.DataEntrega.Should().Be(updatedEntrega.DataEntrega);
            entregaInDb.Destino.Should().Be(updatedEntrega.Destino);
            entregaInDb.Status.Should().Be(updatedEntrega.Status);
        }

        [Fact]
        public async Task DeleteEntrega_ShouldReturnNoContentResponse_WhenEntregaExists()
        {
            // Arrange
            var entrega = _context.Entregas.First();
            var response = await _client.DeleteAsync($"/api/entrega/{entrega.Id}");

            // Act
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Verifying the deletion in the database
            var entregaInDb = await _context.Entregas.FindAsync(entrega.Id);
            entregaInDb.Should().BeNull();
        }
    }
}
