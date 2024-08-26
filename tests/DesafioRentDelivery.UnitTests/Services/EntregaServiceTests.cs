// UnitTests/Services/EntregaServiceTests.cs

using DesafioRentDelivery.Application.DTOs;
using DesafioRentDelivery.Application.Services;
using DesafioRentDelivery.Domain.Entities;
using DesafioRentDelivery.Domain.Repositories;
using DesafioRentDelivery.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DesafioRentDelivery.UnitTests.Services
{
    public class EntregaServiceTests
    {
        private readonly Mock<IEntregaRepository> _entregaRepositoryMock;
        private readonly Mock<IAluguelRepository> _aluguelRepositoryMock;
        private readonly Mock<IRabbitMqService> _rabbitMqServiceMock;
        private readonly EntregaService _entregaService;

        public EntregaServiceTests()
        {
            _entregaRepositoryMock = new Mock<IEntregaRepository>();
            _aluguelRepositoryMock = new Mock<IAluguelRepository>();
            _rabbitMqServiceMock = new Mock<IRabbitMqService>();
            _entregaService = new EntregaService(
                _entregaRepositoryMock.Object,
                _aluguelRepositoryMock.Object,
                _rabbitMqServiceMock.Object,
                Mock.Of<ILogger<EntregaService>>());
        }

        [Fact]
        public async Task AddEntregaAsync_ShouldCallRepositoryAndSendMessage_WhenEntregaIsValid()
        {
            // Arrange
            var entregadorId = 1;
            var entregaDto = new EntregaDTO { EntregadorId = entregadorId, DataEntrega = DateTime.Now, Destino = "Rua A" };
            var aluguelAtivo = new Aluguel { Id = 1, EntregadorId = entregadorId, MotoId = 1, DataInicio = DateTime.Now.AddDays(-1), DataFim = DateTime.Now.AddDays(1) };

            _aluguelRepositoryMock.Setup(x => x.GetAluguelAtivoByEntregadorIdAsync(entregadorId)).ReturnsAsync(aluguelAtivo);

            // Act
            await _entregaService.AddEntregaAsync(entregaDto);

            // Assert
            _entregaRepositoryMock.Verify(x => x.AddEntregaAsync(It.Is<Entrega>(e =>
                e.EntregadorId == entregaDto.EntregadorId && e.Destino == entregaDto.Destino && e.Status == "Pendente")), Times.Once);
            _rabbitMqServiceMock.Verify(x => x.SendMessage(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task AddEntregaAsync_ShouldThrowException_WhenEntregadorHasNoActiveAluguel()
        {
            // Arrange
            var entregadorId = 1;
            var entregaDto = new EntregaDTO { EntregadorId = entregadorId, DataEntrega = DateTime.Now, Destino = "Rua A" };

            _aluguelRepositoryMock.Setup(x => x.GetAluguelAtivoByEntregadorIdAsync(entregadorId)).ReturnsAsync((Aluguel)null);

            // Act
            Func<Task> act = async () => await _entregaService.AddEntregaAsync(entregaDto);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Entregador não tem um aluguel de moto ativo.");
            _entregaRepositoryMock.Verify(x => x.AddEntregaAsync(It.IsAny<Entrega>()), Times.Never);
            _rabbitMqServiceMock.Verify(x => x.SendMessage(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task GetEntregaByIdAsync_ShouldReturnEntrega_WhenEntregaExists()
        {
            // Arrange
            var entregaId = 1;
            var entrega = new Entrega { Id = entregaId, EntregadorId = 1, DataEntrega = DateTime.Now, Destino = "Rua A", Status = "Pendente" };
            _entregaRepositoryMock.Setup(x => x.GetEntregaByIdAsync(entregaId)).ReturnsAsync(entrega);

            // Act
            var result = await _entregaService.GetEntregaByIdAsync(entregaId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(entregaId);
        }

        [Fact]
        public async Task GetEntregaByIdAsync_ShouldReturnNull_WhenEntregaDoesNotExist()
        {
            // Arrange
            var entregaId = 1;
            _entregaRepositoryMock.Setup(x => x.GetEntregaByIdAsync(entregaId)).ReturnsAsync((Entrega)null);

            // Act
            var result = await _entregaService.GetEntregaByIdAsync(entregaId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAllEntregasAsync_ShouldReturnAllEntregas()
        {
            // Arrange
            var entregas = new List<Entrega>
            {
                new Entrega { Id = 1, EntregadorId = 1, DataEntrega = DateTime.Now, Destino = "Rua A", Status = "Pendente" },
                new Entrega { Id = 2, EntregadorId = 2, DataEntrega = DateTime.Now, Destino = "Rua B", Status = "Pendente" }
            };
            _entregaRepositoryMock.Setup(x => x.GetAllEntregasAsync()).ReturnsAsync(entregas);

            // Act
            var result = await _entregaService.GetAllEntregasAsync();

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(e => e.Id == 1);
            result.Should().Contain(e => e.Id == 2);
        }

        [Fact]
        public async Task UpdateEntregaAsync_ShouldCallRepository_WhenEntregaExists()
        {
            // Arrange
            var entregaDto = new EntregaDTO { Id = 1, EntregadorId = 1, DataEntrega = DateTime.Now, Destino = "Rua A", Status = "Concluída" };
            var existingEntrega = new Entrega { Id = 1, EntregadorId = 1, DataEntrega = DateTime.Now, Destino = "Rua A", Status = "Pendente" };
            _entregaRepositoryMock.Setup(x => x.GetEntregaByIdAsync(entregaDto.Id)).ReturnsAsync(existingEntrega);

            // Act
            await _entregaService.UpdateEntregaAsync(entregaDto);

            // Assert
            _entregaRepositoryMock.Verify(x => x.UpdateEntregaAsync(It.Is<Entrega>(e =>
                e.Id == entregaDto.Id && e.Status == entregaDto.Status)), Times.Once);
        }

        [Fact]
        public async Task UpdateEntregaAsync_ShouldThrowException_WhenEntregaDoesNotExist()
        {
            // Arrange
            var entregaDto = new EntregaDTO { Id = 1, EntregadorId = 1, DataEntrega = DateTime.Now, Destino = "Rua A", Status = "Concluída" };
            _entregaRepositoryMock.Setup(x => x.GetEntregaByIdAsync(entregaDto.Id)).ReturnsAsync((Entrega)null);

            // Act
            Func<Task> act = async () => await _entregaService.UpdateEntregaAsync(entregaDto);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Entrega não encontrada.");
            _entregaRepositoryMock.Verify(x => x.UpdateEntregaAsync(It.IsAny<Entrega>()), Times.Never);
        }

        [Fact]
        public async Task RemoveEntregaAsync_ShouldCallRepository_WhenEntregaExists()
        {
            // Arrange
            var entregaId = 1;
            var existingEntrega = new Entrega { Id = entregaId, EntregadorId = 1, DataEntrega = DateTime.Now, Destino = "Rua A", Status = "Pendente" };
            _entregaRepositoryMock.Setup(x => x.GetEntregaByIdAsync(entregaId)).ReturnsAsync(existingEntrega);

            // Act
            await _entregaService.RemoveEntregaAsync(entregaId);

            // Assert
            _entregaRepositoryMock.Verify(x => x.RemoveEntregaAsync(It.Is<Entrega>(e => e.Id == entregaId)), Times.Once);
        }

        [Fact]
        public async Task RemoveEntregaAsync_ShouldThrowException_WhenEntregaDoesNotExist()
        {
            // Arrange
            var entregaId = 1;
            _entregaRepositoryMock.Setup(x => x.GetEntregaByIdAsync(entregaId)).ReturnsAsync((Entrega)null);

            // Act
            Func<Task> act = async () => await _entregaService.RemoveEntregaAsync(entregaId);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Entrega não encontrada.");
            _entregaRepositoryMock.Verify(x => x.RemoveEntregaAsync(It.IsAny<Entrega>()), Times.Never);
        }
    }
}
