// DesafioRentDelivery.IntegrationTests/Repositories/EntregadorRepositoryTests.cs

using DesafioRentDelivery.Domain.Entities;
using DesafioRentDelivery.Infrastructure.Data;
using DesafioRentDelivery.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Logging;
using DesafioRentDelivery.IntegrationTests.Fixtures;

namespace DesafioRentDelivery.IntegrationTests.Repositories
{
    public class EntregadorRepositoryTests : IClassFixture<ApplicationDbContextFixture>
    {
        private readonly EntregadorRepository _entregadorRepository;
        private readonly ApplicationDbContext _context;

        public EntregadorRepositoryTests(ApplicationDbContextFixture fixture)
        {
            _context = fixture.DbContext;

            var loggerMock = new Mock<ILogger<EntregadorRepository>>();

            _entregadorRepository = new EntregadorRepository(_context, loggerMock.Object);
        }

        [Fact]
        public async Task AddEntregadorAsync_ShouldAddEntregadorToDatabase()
        {
            // Arrange
            var entregador = new Entregador
            {
                Nome = "Teste Entregador",
                Documento = "12345678900",
                Telefone = "11999999999"
            };

            // Act
            await _entregadorRepository.AddEntregadorAsync(entregador);

            // Assert
            var entregadorInDb = await _context.Entregadores.FindAsync(entregador.Id);
            entregadorInDb.Should().NotBeNull();
            entregadorInDb.Nome.Should().Be(entregador.Nome);
        }

        [Fact]
        public async Task GetEntregadorByIdAsync_ShouldReturnEntregador_WhenEntregadorExists()
        {
            // Arrange
            var entregador = _context.Entregadores.First();

            // Act
            var result = await _entregadorRepository.GetEntregadorByIdAsync(entregador.Id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(entregador.Id);
        }

        [Fact]
        public async Task GetEntregadorByIdAsync_ShouldReturnNull_WhenEntregadorDoesNotExist()
        {
            // Arrange
            var invalidId = int.MaxValue;

            // Act
            var result = await _entregadorRepository.GetEntregadorByIdAsync(invalidId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAllEntregadoresAsync_ShouldReturnAllEntregadores()
        {
            // Act
            var result = await _entregadorRepository.GetAllEntregadoresAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
        }

        [Fact]
        public async Task UpdateEntregadorAsync_ShouldUpdateEntregadorInDatabase()
        {
            // Arrange
            var entregador = _context.Entregadores.First();
            entregador.Nome = "Entregador Atualizado";

            // Act
            await _entregadorRepository.UpdateEntregadorAsync(entregador);

            // Assert
            var entregadorInDb = await _context.Entregadores.FindAsync(entregador.Id);
            entregadorInDb.Nome.Should().Be(entregador.Nome);
        }

        [Fact]
        public async Task RemoveEntregadorAsync_ShouldRemoveEntregadorFromDatabase()
        {
            // Arrange
            var entregador = _context.Entregadores.First();

            // Act
            await _entregadorRepository.RemoveEntregadorAsync(entregador);

            // Assert
            var entregadorInDb = await _context.Entregadores.FindAsync(entregador.Id);
            entregadorInDb.Should().BeNull();
        }
    }
}
