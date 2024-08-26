// DesafioRentDelivery.IntegrationTests/Controllers/MotoControllerTests.cs

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
    public class MotoControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly ApplicationDbContext _context;

        public MotoControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            _context = factory.Services.GetService(typeof(ApplicationDbContext)) as ApplicationDbContext;
        }

        [Fact]
        public async Task GetAllMotos_ShouldReturnOkResponseWithMotos()
        {
            // Arrange
            var response = await _client.GetAsync("/api/moto");

            // Act
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var motos = await response.Content.ReadFromJsonAsync<IEnumerable<MotoDTO>>();

            // Assert
            motos.Should().NotBeNull();
            motos.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetMotoById_ShouldReturnOkResponseWithMoto_WhenMotoExists()
        {
            // Arrange
            var moto = _context.Motos.First();
            var response = await _client.GetAsync($"/api/moto/{moto.Id}");

            // Act
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<MotoDTO>();

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(moto.Id);
        }

        [Fact]
        public async Task GetMotoById_ShouldReturnNotFound_WhenMotoDoesNotExist()
        {
            // Arrange
            var invalidId = int.MaxValue;
            var response = await _client.GetAsync($"/api/moto/{invalidId}");

            // Act
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CreateMoto_ShouldReturnCreatedResponse_WhenMotoIsValid()
        {
            // Arrange
            var novaMoto = new MotoDTO
            {
                Placa = "XYZ-9876",
                Modelo = "Honda CB500",
                Chassi = "12345678901234567"
            };

            var response = await _client.PostAsJsonAsync("/api/moto", novaMoto);

            // Act
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var createdMoto = await response.Content.ReadFromJsonAsync<MotoDTO>();

            // Assert
            createdMoto.Should().NotBeNull();
            createdMoto.Id.Should().NotBe(0);
            createdMoto.Placa.Should().Be(novaMoto.Placa);
            createdMoto.Modelo.Should().Be(novaMoto.Modelo);
            createdMoto.Chassi.Should().Be(novaMoto.Chassi);
        }

        [Fact]
        public async Task UpdateMoto_ShouldReturnNoContentResponse_WhenMotoIsValid()
        {
            // Arrange
            var moto = _context.Motos.First();
            var updatedMoto = new MotoDTO
            {
                Id = moto.Id,
                Placa = "ABC-1234",
                Modelo = "Yamaha MT-03",
                Chassi = moto.Chassi
            };

            var response = await _client.PutAsJsonAsync($"/api/moto/{moto.Id}", updatedMoto);

            // Act
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Verifying the update in the database
            var motoInDb = await _context.Motos.FindAsync(moto.Id);
            motoInDb.Modelo.Should().Be(updatedMoto.Modelo);
            motoInDb.Placa.Should().Be(updatedMoto.Placa);
        }

        [Fact]
        public async Task DeleteMoto_ShouldReturnNoContentResponse_WhenMotoExists()
        {
            // Arrange
            var moto = _context.Motos.First();
            var response = await _client.DeleteAsync($"/api/moto/{moto.Id}");

            // Act
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Verifying the deletion in the database
            var motoInDb = await _context.Motos.FindAsync(moto.Id);
            motoInDb.Should().BeNull();
        }
    }
}
