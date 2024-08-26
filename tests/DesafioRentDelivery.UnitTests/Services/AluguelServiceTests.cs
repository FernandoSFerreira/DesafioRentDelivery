// UnitTests/Services/AluguelServiceTests.cs

using DesafioRentDelivery.Application.DTOs;
using DesafioRentDelivery.Application.Services;
using DesafioRentDelivery.Domain.Entities;
using DesafioRentDelivery.Domain.Repositories;
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
    public class AluguelServiceTests
    {
        private readonly Mock<IAluguelRepository> _aluguelRepositoryMock;
        private readonly Mock<IEntregadorRepository> _entregadorRepositoryMock;
        private readonly Mock<IMotoRepository> _motoRepositoryMock;
        private readonly AluguelService _aluguelService;

        public AluguelServiceTests()
        {
            _aluguelRepositoryMock = new Mock<IAluguelRepository>();
            _entregadorRepositoryMock = new Mock<IEntregadorRepository>();
            _motoRepositoryMock = new Mock<IMotoRepository>();
            _aluguelService = new AluguelService(
                _aluguelRepositoryMock.Object,
                _entregadorRepositoryMock.Object,
                _motoRepositoryMock.Object,
                Mock.Of<ILogger<AluguelService>>());
        }

        [Fact]
        public async Task AddAluguelAsync_ShouldCallRepository_WhenAluguelIsValid()
        {
            // Arrange
            var entregadorId = 1;
            var motoId = 1;
            var aluguelDto = new AluguelDTO { EntregadorId = entregadorId, MotoId = motoId, DataInicio = DateTime.Now, DataFim = DateTime.Now.AddDays(1) };
            var entregador = new Entregador { Id = entregadorId, Nome = "John Doe" };
            var moto = new Moto { Id = motoId, Placa = "ABC-1234" };

            _entregadorRepositoryMock.Setup(x => x.GetEntregadorByIdAsync(entregadorId)).ReturnsAsync(entregador);
            _motoRepositoryMock.Setup(x => x.GetMotoByIdAsync(motoId)).ReturnsAsync(moto);

            // Act
            await _aluguelService.AddAluguelAsync(aluguelDto);

            // Assert
            _aluguelRepositoryMock.Verify(x => x.AddAluguelAsync(It.Is<Aluguel>(a =>
                a.EntregadorId == entregadorId && a.MotoId == motoId && a.DataInicio == aluguelDto.DataInicio && a.DataFim == aluguelDto.DataFim)), Times.Once);
        }

        [Fact]
        public async Task AddAluguelAsync_ShouldThrowException_WhenEntregadorDoesNotExist()
        {
            // Arrange
            var entregadorId = 1;
            var motoId = 1;
            var aluguelDto = new AluguelDTO { EntregadorId = entregadorId, MotoId = motoId, DataInicio = DateTime.Now, DataFim = DateTime.Now.AddDays(1) };
            _entregadorRepositoryMock.Setup(x => x.GetEntregadorByIdAsync(entregadorId)).ReturnsAsync((Entregador)null);

            // Act
            Func<Task> act = async () => await _aluguelService.AddAluguelAsync(aluguelDto);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("Entregador não encontrado.");
            _aluguelRepositoryMock.Verify(x => x.AddAluguelAsync(It.IsAny<Aluguel>()), Times.Never);
        }

        [Fact]
        public async Task AddAluguelAsync_ShouldThrowException_WhenMotoDoesNotExist()
        {
            // Arrange
            var entregadorId = 1;
            var motoId = 1;
            var aluguelDto = new AluguelDTO { EntregadorId = entregadorId, MotoId = motoId, DataInicio = DateTime.Now, DataFim = DateTime.Now.AddDays(1) };
            var entregador = new Entregador { Id = entregadorId, Nome = "John Doe" };
            _entregadorRepositoryMock.Setup(x => x.GetEntregadorByIdAsync(entregadorId)).ReturnsAsync(entregador);
            _motoRepositoryMock.Setup(x => x.GetMotoByIdAsync(motoId)).ReturnsAsync((Moto)null);

            // Act
            Func<Task> act = async () => await _aluguelService.AddAluguelAsync(aluguelDto);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("Moto não encontrada.");
            _aluguelRepositoryMock.Verify(x => x.AddAluguelAsync(It.IsAny<Aluguel>()), Times.Never);
        }

        [Fact]
        public async Task GetAluguelByIdAsync_ShouldReturnAluguel_WhenAluguelExists()
        {
            // Arrange
            var aluguelId = 1;
            var aluguel = new Aluguel { Id = aluguelId, EntregadorId = 1, MotoId = 1, DataInicio = DateTime.Now, DataFim = DateTime.Now.AddDays(1) };
            _aluguelRepositoryMock.Setup(x => x.GetAluguelByIdAsync(aluguelId)).ReturnsAsync(aluguel);

            // Act
            var result = await _aluguelService.GetAluguelByIdAsync(aluguelId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(aluguelId);
        }

        [Fact]
        public async Task GetAluguelByIdAsync_ShouldReturnNull_WhenAluguelDoesNotExist()
        {
            // Arrange
            var aluguelId = 1;
            _aluguelRepositoryMock.Setup(x => x.GetAluguelByIdAsync(aluguelId)).ReturnsAsync((Aluguel)null);

            // Act
            var result = await _aluguelService.GetAluguelByIdAsync(aluguelId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAllAlugueisAsync_ShouldReturnAllAlugueis()
        {
            // Arrange
            var alugueis = new List<Aluguel>
            {
                new Aluguel { Id = 1, EntregadorId = 1, MotoId = 1, DataInicio = DateTime.Now, DataFim = DateTime.Now.AddDays(1) },
                new Aluguel { Id = 2, EntregadorId = 2, MotoId = 2, DataInicio = DateTime.Now, DataFim = DateTime.Now.AddDays(1) }
            };
            _aluguelRepositoryMock.Setup(x => x.GetAllAlugueisAsync()).ReturnsAsync(alugueis);

            // Act
            var result = await _aluguelService.GetAllAlugueisAsync();

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(a => a.Id == 1);
            result.Should().Contain(a => a.Id == 2);
        }

        [Fact]
        public async Task GetAlugueisByEntregadorIdAsync_ShouldReturnAlugueis_WhenEntregadorHasAlugueis()
        {
            // Arrange
            var entregadorId = 1;
            var alugueis = new List<Aluguel>
            {
                new Aluguel { Id = 1, EntregadorId = entregadorId, MotoId = 1, DataInicio = DateTime.Now, DataFim = DateTime.Now.AddDays(1) },
                new Aluguel { Id = 2, EntregadorId = entregadorId, MotoId = 2, DataInicio = DateTime.Now, DataFim = DateTime.Now.AddDays(1) }
            };
            _aluguelRepositoryMock.Setup(x => x.GetAlugueisByEntregadorIdAsync(entregadorId)).ReturnsAsync(alugueis);

            // Act
            var result = await _aluguelService.GetAlugueisByEntregadorIdAsync(entregadorId);

            // Assert
            result.Should().HaveCount(2);
            result.Should().AllBeAssignableTo<AluguelDTO>();
            result.Should().Contain(a => a.Id == 1);
            result.Should().Contain(a => a.Id == 2);
        }

        [Fact]
        public async Task GetAluguelAtivoByEntregadorIdAsync_ShouldReturnAluguel_WhenEntregadorHasActiveAluguel()
        {
            // Arrange
            var entregadorId = 1;
            var aluguel = new Aluguel { Id = 1, EntregadorId = entregadorId, MotoId = 1, DataInicio = DateTime.Now, DataFim = DateTime.Now.AddDays(1) };
            _aluguelRepositoryMock.Setup(x => x.GetAluguelAtivoByEntregadorIdAsync(entregadorId)).ReturnsAsync(aluguel);

            // Act
            var result = await _aluguelService.GetAluguelAtivoByEntregadorIdAsync(entregadorId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(aluguel.Id);
            result.MotoId.Should().Be(aluguel.MotoId);
            result.DataInicio.Should().Be(aluguel.DataInicio);
            result.DataFim.Should().Be(aluguel.DataFim);
        }

        [Fact]
        public async Task GetAluguelAtivoByEntregadorIdAsync_ShouldReturnNull_WhenEntregadorDoesNotHaveActiveAluguel()
        {
            // Arrange
            var entregadorId = 1;
            _aluguelRepositoryMock.Setup(x => x.GetAluguelAtivoByEntregadorIdAsync(entregadorId)).ReturnsAsync((Aluguel)null);

            // Act
            var result = await _aluguelService.GetAluguelAtivoByEntregadorIdAsync(entregadorId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task UpdateAluguelAsync_ShouldCallRepository_WhenAluguelExists()
        {
            // Arrange
            var aluguelDto = new AluguelDTO { Id = 1, EntregadorId = 1, MotoId = 1, DataInicio = DateTime.Now, DataFim = DateTime.Now.AddDays(1) };
            var existingAluguel = new Aluguel { Id = 1, EntregadorId = 1, MotoId = 1, DataInicio = DateTime.Now, DataFim = DateTime.Now.AddDays(1) };
            _aluguelRepositoryMock.Setup(x => x.GetAluguelByIdAsync(aluguelDto.Id)).ReturnsAsync(existingAluguel);

            // Act
            await _aluguelService.UpdateAluguelAsync(aluguelDto);

            // Assert
            _aluguelRepositoryMock.Verify(x => x.UpdateAluguelAsync(It.Is<Aluguel>(a =>
                a.Id == aluguelDto.Id && a.EntregadorId == aluguelDto.EntregadorId && a.MotoId == aluguelDto.MotoId &&
                a.DataInicio == aluguelDto.DataInicio && a.DataFim == aluguelDto.DataFim)), Times.Once);
        }

        [Fact]
        public async Task UpdateAluguelAsync_ShouldThrowException_WhenAluguelDoesNotExist()
        {
            // Arrange
            var aluguelDto = new AluguelDTO { Id = 1, EntregadorId = 1, MotoId = 1, DataInicio = DateTime.Now, DataFim = DateTime.Now.AddDays(1) };
            _aluguelRepositoryMock.Setup(x => x.GetAluguelByIdAsync(aluguelDto.Id)).ReturnsAsync((Aluguel)null);

            // Act
            Func<Task> act = async () => await _aluguelService.UpdateAluguelAsync(aluguelDto);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("Aluguel não encontrado.");
            _aluguelRepositoryMock.Verify(x => x.UpdateAluguelAsync(It.IsAny<Aluguel>()), Times.Never);
        }

        [Fact]
        public async Task RemoveAluguelAsync_ShouldCallRepository_WhenAluguelExists()
        {
            // Arrange
            var aluguelId = 1;
            var existingAluguel = new Aluguel { Id = aluguelId, EntregadorId = 1, MotoId = 1, DataInicio = DateTime.Now, DataFim = DateTime.Now.AddDays(1) };
            _aluguelRepositoryMock.Setup(x => x.GetAluguelByIdAsync(aluguelId)).ReturnsAsync(existingAluguel);

            // Act
            await _aluguelService.RemoveAluguelAsync(aluguelId);

            // Assert
            _aluguelRepositoryMock.Verify(x => x.RemoveAluguelAsync(It.Is<Aluguel>(a => a.Id == aluguelId)), Times.Once);
        }

        [Fact]
        public async Task RemoveAluguelAsync_ShouldThrowException_WhenAluguelDoesNotExist()
        {
            // Arrange
            var aluguelId = 1;
            _aluguelRepositoryMock.Setup(x => x.GetAluguelByIdAsync(aluguelId)).ReturnsAsync((Aluguel)null);

            // Act
            Func<Task> act = async () => await _aluguelService.RemoveAluguelAsync(aluguelId);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("Aluguel não encontrado.");
            _aluguelRepositoryMock.Verify(x => x.RemoveAluguelAsync(It.IsAny<Aluguel>()), Times.Never);
        }
    }
}
