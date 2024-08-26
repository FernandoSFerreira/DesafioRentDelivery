// DesafioRentDelivery.IntegrationTests/Controllers/AluguelControllerTests.cs

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
    public class AluguelControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly ApplicationDbContext _context;

        public AluguelControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            _context = factory.Services.GetService(typeof(ApplicationDbContext)) as ApplicationDbContext;
        }

        [Fact]
        public async Task GetAllAlugueis_ShouldReturnOkResponseWithAlugueis()
        {
            // Arrange
            var response = await _client.GetAsync("/api/aluguel");

            // Act
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var alugueis = await response.Content.ReadFromJsonAsync<IEnumerable<AluguelDTO>>();

            // Assert
            alugueis.Should().NotBeNull();
            alugueis.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetAluguelById_ShouldReturnOkResponseWithAluguel_WhenAluguelExists()
        {
            // Arrange
            var aluguel = _context.Alugueis.First();
            var response = await _client.GetAsync($"/api/aluguel/{aluguel.Id}");

            // Act
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<AluguelDTO>();

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(aluguel.Id);
        }

        [Fact]
        public async Task GetAluguelById_ShouldReturnNotFound_WhenAluguelDoesNotExist()
        {
            // Arrange
            var invalidId = int.MaxValue;
            var response = await _client.GetAsync($"/api/aluguel/{invalidId}");

            // Act
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CreateAluguel_ShouldReturnCreatedResponse_WhenAluguelIsValid()
        {
            // Arrange
            var entregador = _context.Entregadores.First();
            var moto = _context.Motos.First();
            var novoAluguel = new AluguelDTO
            {
                EntregadorId = entregador.Id,
                MotoId = moto.Id,
                DataInicio = DateTime.Now,
                DataFim = DateTime.Now.AddDays(1)
            };

            var response = await _client.PostAsJsonAsync("/api/aluguel", novoAluguel);

            // Act
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var createdAluguel = await response.Content.ReadFromJsonAsync<AluguelDTO>();

            // Assert
            createdAluguel.Should().NotBeNull();
            createdAluguel.Id.Should().NotBe(0);
            createdAluguel.EntregadorId.Should().Be(novoAluguel.EntregadorId);
            createdAluguel.MotoId.Should().Be(novoAluguel.MotoId);
        }

        [Fact]
        public async Task UpdateAluguel_ShouldReturnNoContentResponse_WhenAluguelIsValid()
        {
            // Arrange
            var aluguel = _context.Alugueis.First();
            var updatedAluguel = new AluguelDTO
            {
                Id = aluguel.Id,
                EntregadorId = aluguel.EntregadorId,
                MotoId = aluguel.MotoId,
                DataInicio = aluguel.DataInicio.AddDays(1),
                DataFim = aluguel.DataFim.AddDays(1)
            };

            var response = await _client.PutAsJsonAsync($"/api/aluguel/{aluguel.Id}", updatedAluguel);

            // Act
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Verifying the update in the database
            var aluguelInDb = await _context.Alugueis.FindAsync(aluguel.Id);
            aluguelInDb.DataInicio.Should().Be(updatedAluguel.DataInicio);
            aluguelInDb.DataFim.Should().Be(updatedAluguel.DataFim);
        }

        [Fact]
        public async Task DeleteAluguel_ShouldReturnNoContentResponse_WhenAluguelExists()
        {
            // Arrange
            var aluguel = _context.Alugueis.First();
            var response = await _client.DeleteAsync($"/api/aluguel/{aluguel.Id}");

            // Act
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Verifying the deletion in the database
            var aluguelInDb = await _context.Alugueis.FindAsync(aluguel.Id);
            aluguelInDb.Should().BeNull();
        }
    }
}
