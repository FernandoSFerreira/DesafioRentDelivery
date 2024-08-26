// UnitTests/Services/EntregadorServiceTests.cs

using DesafioRentDelivery.Application.DTOs;
using DesafioRentDelivery.Application.Services;
using DesafioRentDelivery.Domain.Entities;
using DesafioRentDelivery.Domain.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DesafioRentDelivery.UnitTests.Services
{
    public class EntregadorServiceTests
    {
        private readonly Mock<IEntregadorRepository> _entregadorRepositoryMock;
        private readonly Mock<IAluguelRepository> _aluguelRepositoryMock;
        private readonly Mock<IEntregaRepository> _entregaRepositoryMock;
        private readonly EntregadorService _entregadorService;

        public EntregadorServiceTests()
        {
            _entregadorRepositoryMock = new Mock<IEntregadorRepository>();
            _aluguelRepositoryMock = new Mock<IAluguelRepository>();
            _entregaRepositoryMock = new Mock<IEntregaRepository>();
            _entregadorService = new EntregadorService(
                _entregadorRepositoryMock.Object,
                _aluguelRepositoryMock.Object,
                _entregaRepositoryMock.Object,
                Mock.Of<ILogger<EntregadorService>>());
        }

        [Fact]
        public async Task AddEntregadorAsync_ShouldCallRepository_WhenEntregadorIsValid()
        {
            // Arrange
            var entregadorDto = new EntregadorDTO { Nome = "John Doe", Documento = "123456789", Telefone = "987654321" };

            // Act
            await _entregadorService.AddEntregadorAsync(entregadorDto);

            // Assert
            _entregadorRepositoryMock.Verify(x => x.AddEntregadorAsync(It.Is<Entregador>(e =>
                e.Nome == entregadorDto.Nome && e.Documento == entregadorDto.Documento && e.Telefone == entregadorDto.Telefone)), Times.Once);
        }

        [Fact]
        public async Task GetEntregadorByIdAsync_ShouldReturnEntregador_WhenEntregadorExists()
        {
            // Arrange
            var entregadorId = 1;
            var entregador = new Entregador { Id = entregadorId, Nome = "John Doe", Documento = "123456789", Telefone = "987654321" };
            _entregadorRepositoryMock.Setup(x => x.GetEntregadorByIdAsync(entregadorId)).ReturnsAsync(entregador);

            // Act
            var result = await _entregadorService.GetEntregadorByIdAsync(entregadorId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(entregadorId);
            result.Nome.Should().Be("John Doe");
        }

        [Fact]
        public async Task GetEntregadorByIdAsync_ShouldReturnNull_WhenEntregadorDoesNotExist()
        {
            // Arrange
            var entregadorId = 1;
            _entregadorRepositoryMock.Setup(x => x.GetEntregadorByIdAsync(entregadorId)).ReturnsAsync((Entregador)null);

            // Act
            var result = await _entregadorService.GetEntregadorByIdAsync(entregadorId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAllEntregadoresAsync_ShouldReturnAllEntregadores()
        {
            // Arrange
            var entregadores = new List<Entregador>
            {
                new Entregador { Id = 1, Nome = "John Doe", Documento = "123456789", Telefone = "987654321" },
                new Entregador { Id = 2, Nome = "Jane Smith", Documento = "987654321", Telefone = "123456789" }
            };
            _entregadorRepositoryMock.Setup(x => x.GetAllEntregadoresAsync()).ReturnsAsync(entregadores);

            // Act
            var result = await _entregadorService.GetAllEntregadoresAsync();

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(e => e.Nome == "John Doe");
            result.Should().Contain(e => e.Nome == "Jane Smith");
        }

        [Fact]
        public async Task UpdateEntregadorAsync_ShouldCallRepository_WhenEntregadorExists()
        {
            // Arrange
            var entregadorDto = new EntregadorDTO { Id = 1, Nome = "John Doe", Documento = "123456789", Telefone = "987654321" };
            var existingEntregador = new Entregador { Id = 1, Nome = "John Doe", Documento = "123456789", Telefone = "987654321" };
            _entregadorRepositoryMock.Setup(x => x.GetEntregadorByIdAsync(entregadorDto.Id)).ReturnsAsync(existingEntregador);

            // Act
            await _entregadorService.UpdateEntregadorAsync(entregadorDto);

            // Assert
            _entregadorRepositoryMock.Verify(x => x.UpdateEntregadorAsync(It.Is<Entregador>(e =>
                e.Id == entregadorDto.Id && e.Nome == entregadorDto.Nome && e.Documento == entregadorDto.Documento && e.Telefone == entregadorDto.Telefone)), Times.Once);
        }

        [Fact]
        public async Task UpdateEntregadorAsync_ShouldNotCallRepository_WhenEntregadorDoesNotExist()
        {
            // Arrange
            var entregadorDto = new EntregadorDTO { Id = 1, Nome = "John Doe", Documento = "123456789", Telefone = "987654321" };
            _entregadorRepositoryMock.Setup(x => x.GetEntregadorByIdAsync(entregadorDto.Id)).ReturnsAsync((Entregador)null);

            // Act
            await _entregadorService.UpdateEntregadorAsync(entregadorDto);

            // Assert
            _entregadorRepositoryMock.Verify(x => x.UpdateEntregadorAsync(It.IsAny<Entregador>()), Times.Never);
        }

        [Fact]
        public async Task RemoveEntregadorAsync_ShouldCallRepository_WhenEntregadorExists()
        {
            // Arrange
            var entregadorId = 1;
            var existingEntregador = new Entregador { Id = entregadorId, Nome = "John Doe", Documento = "123456789", Telefone = "987654321" };
            _entregadorRepositoryMock.Setup(x => x.GetEntregadorByIdAsync(entregadorId)).ReturnsAsync(existingEntregador);

            // Act
            await _entregadorService.RemoveEntregadorAsync(entregadorId);

            // Assert
            _entregadorRepositoryMock.Verify(x => x.RemoveEntregadorAsync(It.Is<Entregador>(e => e.Id == entregadorId)), Times.Once);
        }

        [Fact]
        public async Task RemoveEntregadorAsync_ShouldNotCallRepository_WhenEntregadorDoesNotExist()
        {
            // Arrange
            var entregadorId = 1;
            _entregadorRepositoryMock.Setup(x => x.GetEntregadorByIdAsync(entregadorId)).ReturnsAsync((Entregador)null);

            // Act
            await _entregadorService.RemoveEntregadorAsync(entregadorId);

            // Assert
            _entregadorRepositoryMock.Verify(x => x.RemoveEntregadorAsync(It.IsAny<Entregador>()), Times.Never);
        }
    }
}
