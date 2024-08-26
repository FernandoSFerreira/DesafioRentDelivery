// UnitTests/Services/MotoServiceTests.cs

using DesafioRentDelivery.Application.DTOs;
using DesafioRentDelivery.Application.Services;
using DesafioRentDelivery.Domain.Entities;
using DesafioRentDelivery.Domain.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace DesafioRentDelivery.UnitTests.Services
{
    public class MotoServiceTests
    {
        private readonly Mock<IMotoRepository> _motoRepositoryMock;
        private readonly MotoService _motoService;

        public MotoServiceTests()
        {
            _motoRepositoryMock = new Mock<IMotoRepository>();
            _motoService = new MotoService(_motoRepositoryMock.Object, Mock.Of<ILogger<MotoService>>());
        }

        [Fact]
        public async Task AddMotoAsync_ShouldCallRepository_WhenMotoIsValid()
        {
            // Arrange
            var motoDto = new MotoDTO { Placa = "ABC-1234", Modelo = "Honda", Chassi = "XYZ789" };

            // Act
            await _motoService.AddMotoAsync(motoDto);

            // Assert
            _motoRepositoryMock.Verify(x => x.AddMotoAsync(It.Is<Moto>(m =>
                m.Placa == motoDto.Placa && m.Modelo == motoDto.Modelo && m.Chassi == motoDto.Chassi)), Times.Once);
        }

        [Fact]
        public async Task GetMotoByIdAsync_ShouldReturnMoto_WhenMotoExists()
        {
            // Arrange
            var motoId = 1;
            var moto = new Moto { Id = motoId, Placa = "ABC-1234", Modelo = "Honda", Chassi = "XYZ789" };
            _motoRepositoryMock.Setup(x => x.GetMotoByIdAsync(motoId)).ReturnsAsync(moto);

            // Act
            var result = await _motoService.GetMotoByIdAsync(motoId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(motoId);
            result.Placa.Should().Be("ABC-1234");
        }

        [Fact]
        public async Task GetMotoByIdAsync_ShouldReturnNull_WhenMotoDoesNotExist()
        {
            // Arrange
            var motoId = 1;
            _motoRepositoryMock.Setup(x => x.GetMotoByIdAsync(motoId)).ReturnsAsync((Moto)null);

            // Act
            var result = await _motoService.GetMotoByIdAsync(motoId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAllMotosAsync_ShouldReturnAllMotos()
        {
            // Arrange
            var motos = new List<Moto>
            {
                new Moto { Id = 1, Placa = "ABC-1234", Modelo = "Honda", Chassi = "XYZ789" },
                new Moto { Id = 2, Placa = "DEF-5678", Modelo = "Yamaha", Chassi = "UVW123" }
            };
            _motoRepositoryMock.Setup(x => x.GetAllMotosAsync()).ReturnsAsync(motos);

            // Act
            var result = await _motoService.GetAllMotosAsync();

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(m => m.Placa == "ABC-1234");
            result.Should().Contain(m => m.Placa == "DEF-5678");
        }

        [Fact]
        public async Task UpdateMotoAsync_ShouldCallRepository_WhenMotoExists()
        {
            // Arrange
            var motoDto = new MotoDTO { Id = 1, Placa = "ABC-1234", Modelo = "Honda", Chassi = "XYZ789" };
            var existingMoto = new Moto { Id = 1, Placa = "ABC-1234", Modelo = "Honda", Chassi = "XYZ789" };
            _motoRepositoryMock.Setup(x => x.GetMotoByIdAsync(motoDto.Id)).ReturnsAsync(existingMoto);

            // Act
            await _motoService.UpdateMotoAsync(motoDto);

            // Assert
            _motoRepositoryMock.Verify(x => x.UpdateMotoAsync(It.Is<Moto>(m =>
                m.Id == motoDto.Id && m.Placa == motoDto.Placa && m.Modelo == motoDto.Modelo && m.Chassi == motoDto.Chassi)), Times.Once);
        }

        [Fact]
        public async Task UpdateMotoAsync_ShouldNotCallRepository_WhenMotoDoesNotExist()
        {
            // Arrange
            var motoDto = new MotoDTO { Id = 1, Placa = "ABC-1234", Modelo = "Honda", Chassi = "XYZ789" };
            _motoRepositoryMock.Setup(x => x.GetMotoByIdAsync(motoDto.Id)).ReturnsAsync((Moto)null);

            // Act
            await _motoService.UpdateMotoAsync(motoDto);

            // Assert
            _motoRepositoryMock.Verify(x => x.UpdateMotoAsync(It.IsAny<Moto>()), Times.Never);
        }

        [Fact]
        public async Task RemoveMotoAsync_ShouldCallRepository_WhenMotoExists()
        {
            // Arrange
            var motoId = 1;
            var existingMoto = new Moto { Id = motoId, Placa = "ABC-1234", Modelo = "Honda", Chassi = "XYZ789" };
            _motoRepositoryMock.Setup(x => x.GetMotoByIdAsync(motoId)).ReturnsAsync(existingMoto);

            // Act
            await _motoService.RemoveMotoAsync(motoId);

            // Assert
            _motoRepositoryMock.Verify(x => x.RemoveMotoAsync(It.Is<Moto>(m => m.Id == motoId)), Times.Once);
        }

        [Fact]
        public async Task RemoveMotoAsync_ShouldNotCallRepository_WhenMotoDoesNotExist()
        {
            // Arrange
            var motoId = 1;
            _motoRepositoryMock.Setup(x => x.GetMotoByIdAsync(motoId)).ReturnsAsync((Moto)null);

            // Act
            await _motoService.RemoveMotoAsync(motoId);

            // Assert
            _motoRepositoryMock.Verify(x => x.RemoveMotoAsync(It.IsAny<Moto>()), Times.Never);
        }

        [Fact]
        public async Task AddHistoricoManutencaoAsync_ShouldCallRepository_WhenHistoricoIsValid()
        {
            // Arrange
            var historicoDto = new HistoricoManutencaoDTO { DataManutencao = System.DateTime.Now, Descricao = "Troca de óleo" };
            var motoId = 1;

            // Act
            await _motoService.AddHistoricoManutencaoAsync(motoId, historicoDto);

            // Assert
            _motoRepositoryMock.Verify(x => x.AddHistoricoManutencaoAsync(It.Is<HistoricoManutencao>(h =>
                h.MotoId == motoId && h.Descricao == historicoDto.Descricao)), Times.Once);
        }

        [Fact]
        public async Task RemoveHistoricoManutencaoAsync_ShouldCallRepository_WhenHistoricoExists()
        {
            // Arrange
            var historicoId = 1;

            // Act
            await _motoService.RemoveHistoricoManutencaoAsync(historicoId);

            // Assert
            _motoRepositoryMock.Verify(x => x.RemoveHistoricoManutencaoAsync(It.Is<HistoricoManutencao>(h => h.Id == historicoId)), Times.Once);
        }
    }
}
