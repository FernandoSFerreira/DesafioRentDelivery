// DesafioRentDelivery.IntegrationTests/Repositories/MotoRepositoryTests.cs

using DesafioRentDelivery.Domain.Entities;
using DesafioRentDelivery.Infrastructure.Data;
using DesafioRentDelivery.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Logging;
using DesafioRentDelivery.IntegrationTests.Fixtures;

namespace DesafioRentDelivery.IntegrationTests.Repositories
{
    public class MotoRepositoryTests : IClassFixture<ApplicationDbContextFixture>
    {
        private readonly MotoRepository _motoRepository;
        private readonly ApplicationDbContext _context;

        public MotoRepositoryTests(ApplicationDbContextFixture fixture)
        {
            _context = fixture.DbContext;

            var loggerMock = new Mock<ILogger<MotoRepository>>();

            _motoRepository = new MotoRepository(_context, loggerMock.Object);
        }

        [Fact]
        public async Task AddMotoAsync_ShouldAddMotoToDatabase()
        {
            // Arrange
            var moto = new Moto
            {
                Placa = "XYZ-9876",
                Modelo = "Honda CB500",
                Chassi = "12345678901234567"
            };

            // Act
            await _motoRepository.AddMotoAsync(moto);

            // Assert
            var motoInDb = await _context.Motos.FindAsync(moto.Id);
            motoInDb.Should().NotBeNull();
            motoInDb.Placa.Should().Be(moto.Placa);
        }

        [Fact]
        public async Task GetMotoByIdAsync_ShouldReturnMoto_WhenMotoExists()
        {
            // Arrange
            var moto = _context.Motos.First();

            // Act
            var result = await _motoRepository.GetMotoByIdAsync(moto.Id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(moto.Id);
        }

        [Fact]
        public async Task GetMotoByIdAsync_ShouldReturnNull_WhenMotoDoesNotExist()
        {
            // Arrange
            var invalidId = int.MaxValue;

            // Act
            var result = await _motoRepository.GetMotoByIdAsync(invalidId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAllMotosAsync_ShouldReturnAllMotos()
        {
            // Act
            var result = await _motoRepository.GetAllMotosAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
        }

        [Fact]
        public async Task UpdateMotoAsync_ShouldUpdateMotoInDatabase()
        {
            // Arrange
            var moto = _context.Motos.First();
            moto.Modelo = "Yamaha MT-03";

            // Act
            await _motoRepository.UpdateMotoAsync(moto);

            // Assert
            var motoInDb = await _context.Motos.FindAsync(moto.Id);
            motoInDb.Modelo.Should().Be(moto.Modelo);
        }

        [Fact]
        public async Task RemoveMotoAsync_ShouldRemoveMotoFromDatabase()
        {
            // Arrange
            var moto = _context.Motos.First();

            // Act
            await _motoRepository.RemoveMotoAsync(moto);

            // Assert
            var motoInDb = await _context.Motos.FindAsync(moto.Id);
            motoInDb.Should().BeNull();
        }

        [Fact]
        public async Task AddHistoricoManutencaoAsync_ShouldAddHistoricoToMoto()
        {
            // Arrange
            var moto = _context.Motos.First();
            var historico = new HistoricoManutencao
            {
                MotoId = moto.Id,
                DataManutencao = DateTime.Now,
                Descricao = "Troca de óleo"
            };

            // Act
            await _motoRepository.AddHistoricoManutencaoAsync(historico);

            // Assert
            var motoInDb = await _context.Motos
                .Include(m => m.HistoricoManutencoes)
                .FirstOrDefaultAsync(m => m.Id == moto.Id);

            motoInDb.HistoricoManutencoes.Should().Contain(h => h.Descricao == "Troca de óleo");
        }

        [Fact]
        public async Task RemoveHistoricoManutencaoAsync_ShouldRemoveHistoricoFromMoto()
        {
            // Arrange
            var moto = _context.Motos
                .Include(m => m.HistoricoManutencoes)
                .FirstOrDefault();

            var historico = moto.HistoricoManutencoes.First();

            // Act
            await _motoRepository.RemoveHistoricoManutencaoAsync(historico);

            // Assert
            var motoInDb = await _context.Motos
                .Include(m => m.HistoricoManutencoes)
                .FirstOrDefaultAsync(m => m.Id == moto.Id);

            motoInDb.HistoricoManutencoes.Should().NotContain(h => h.Id == historico.Id);
        }
    }
}
