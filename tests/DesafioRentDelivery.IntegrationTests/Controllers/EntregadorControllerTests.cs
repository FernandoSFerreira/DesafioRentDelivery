// DesafioRentDelivery.IntegrationTests/Controllers/EntregadorControllerTests.cs

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
    public class EntregadorControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly ApplicationDbContext _context;

        public EntregadorControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            _context = factory.Services.GetService(typeof(ApplicationDbContext)) as ApplicationDbContext;
        }

        [Fact]
        public async Task GetAllEntregadores_ShouldReturnOkResponseWithEntregadores()
        {
            // Arrange
            var response = await _client.GetAsync("/api/entregador");

            // Act
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var entregadores = await response.Content.ReadFromJsonAsync<IEnumerable<EntregadorDTO>>();

            // Assert
            entregadores.Should().NotBeNull();
            entregadores.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetEntregadorById_ShouldReturnOkResponseWithEntregador_WhenEntregadorExists()
        {
            // Arrange
            var entregador = _context.Entregadores.First();
            var response = await _client.GetAsync($"/api/entregador/{entregador.Id}");

            // Act
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<EntregadorDTO>();

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(entregador.Id);
        }

        [Fact]
        public async Task GetEntregadorById_ShouldReturnNotFound_WhenEntregadorDoesNotExist()
        {
            // Arrange
            var invalidId = int.MaxValue;
            var response = await _client.GetAsync($"/api/entregador/{invalidId}");

            // Act
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CreateEntregador_ShouldReturnCreatedResponse_WhenEntregadorIsValid()
        {
            // Arrange
            var novoEntregador = new EntregadorDTO
            {
                Nome = "Novo Entregador",
                Documento = "12345678900",
                Telefone = "11999999999"
            };

            var response = await _client.PostAsJsonAsync("/api/entregador", novoEntregador);

            // Act
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var createdEntregador = await response.Content.ReadFromJsonAsync<EntregadorDTO>();

            // Assert
            createdEntregador.Should().NotBeNull();
            createdEntregador.Id.Should().NotBe(0);
            createdEntregador.Nome.Should().Be(novoEntregador.Nome);
            createdEntregador.Documento.Should().Be(novoEntregador.Documento);
            createdEntregador.Telefone.Should().Be(novoEntregador.Telefone);
        }

        [Fact]
        public async Task UpdateEntregador_ShouldReturnNoContentResponse_WhenEntregadorIsValid()
        {
            // Arrange
            var entregador = _context.Entregadores.First();
            var updatedEntregador = new EntregadorDTO
            {
                Id = entregador.Id,
                Nome = "Entregador Atualizado",
                Documento = entregador.Documento,
                Telefone = "11888888888"
            };

            var response = await _client.PutAsJsonAsync($"/api/entregador/{entregador.Id}", updatedEntregador);

            // Act
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Verifying the update in the database
            var entregadorInDb = await _context.Entregadores.FindAsync(entregador.Id);
            entregadorInDb.Nome.Should().Be(updatedEntregador.Nome);
            entregadorInDb.Telefone.Should().Be(updatedEntregador.Telefone);
        }

        [Fact]
        public async Task DeleteEntregador_ShouldReturnNoContentResponse_WhenEntregadorExists()
        {
            // Arrange
            var entregador = _context.Entregadores.First();
            var response = await _client.DeleteAsync($"/api/entregador/{entregador.Id}");

            // Act
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Verifying the deletion in the database
            var entregadorInDb = await _context.Entregadores.FindAsync(entregador.Id);
            entregadorInDb.Should().BeNull();
        }
    }
}
