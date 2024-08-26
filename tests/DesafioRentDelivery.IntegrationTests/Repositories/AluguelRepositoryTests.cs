// DesafioRentDelivery.IntegrationTests/Repositories/AluguelRepositoryTests.cs

using DesafioRentDelivery.Domain.Entities;
using DesafioRentDelivery.Infrastructure.Data;
using DesafioRentDelivery.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
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
    public class AluguelRepositoryTests : IClassFixture<ApplicationDbContextFixture>
    {
        private readonly AluguelRepository _aluguelRepository;
        private readonly ApplicationDbContext _context;
        private readonly IMongoCollection<Aluguel> _mongoCollection;

        public AluguelRepositoryTests(ApplicationDbContextFixture fixture)
        {
            _context = fixture.DbContext;
            _mongoCollection = fixture.MongoDatabase.GetCollection<Aluguel>("Alugueis");

            var loggerMock = new Mock<ILogger<AluguelRepository>>();

            _aluguelRepository = new AluguelRepository(_context, fixture.MongoClient, loggerMock.Object);
        }

        [Fact]
        public async Task AddAluguelAsync_ShouldAddAluguelToBothDatabases()
        {
            // Arrange
            var aluguel = new Aluguel
            {
                EntregadorId = 1,
                MotoId = 1,
                DataInicio = DateTime.Now,
                DataFim = DateTime.Now.AddDays(1)
            };

            // Act
            await _aluguelRepository.AddAluguelAsync(aluguel);

            // Assert - PostgreSQL
            var aluguelInSql = await _context.Alugueis.FindAsync(aluguel.Id);
            aluguelInSql.Should().NotBeNull();

            // Assert - MongoDB
            var filter = Builders<Aluguel>.Filter.Eq("Id", aluguel.Id);
            var aluguelInMongo = await _mongoCollection.Find(filter).FirstOrDefaultAsync();
            aluguelInMongo.Should().NotBeNull();
        }

        [Fact]
        public async Task GetAluguelByIdAsync_ShouldReturnAluguel_WhenAluguelExists()
        {
            // Arrange
            var aluguel = _context.Alugueis.First();

            // Act
            var result = await _aluguelRepository.GetAluguelByIdAsync(aluguel.Id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(aluguel.Id);
        }

        [Fact]
        public async Task GetAllAlugueisAsync_ShouldReturnAllAlugueis()
        {
            // Act
            var result = await _aluguelRepository.GetAllAlugueisAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetAlugueisByEntregadorIdAsync_ShouldReturnAlugueis_WhenEntregadorHasAlugueis()
        {
            // Arrange
            var entregadorId = 1;

            // Act
            var result = await _aluguelRepository.GetAlugueisByEntregadorIdAsync(entregadorId);

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetAluguelAtivoByEntregadorIdAsync_ShouldReturnAluguel_WhenAluguelIsActive()
        {
            // Arrange
            var entregadorId = 1;

            // Act
            var result = await _aluguelRepository.GetAluguelAtivoByEntregadorIdAsync(entregadorId);

            // Assert
            result.Should().NotBeNull();
            result.DataInicio.Should().BeBefore(DateTime.Now);
            result.DataFim.Should().BeAfter(DateTime.Now);
        }

        [Fact]
        public async Task UpdateAluguelAsync_ShouldUpdateAluguelInBothDatabases()
        {
            // Arrange
            var aluguel = _context.Alugueis.First();
            aluguel.DataFim = aluguel.DataFim.AddDays(1);

            // Act
            await _aluguelRepository.UpdateAluguelAsync(aluguel);

            // Assert - PostgreSQL
            var aluguelInSql = await _context.Alugueis.FindAsync(aluguel.Id);
            aluguelInSql.DataFim.Should().Be(aluguel.DataFim);

            // Assert - MongoDB
            var filter = Builders<Aluguel>.Filter.Eq("Id", aluguel.Id);
            var aluguelInMongo = await _mongoCollection.Find(filter).FirstOrDefaultAsync();
            aluguelInMongo.DataFim.Should().Be(aluguel.DataFim);
        }

        [Fact]
        public async Task RemoveAluguelAsync_ShouldRemoveAluguelFromBothDatabases()
        {
            // Arrange
            var aluguel = _context.Alugueis.First();

            // Act
            await _aluguelRepository.RemoveAluguelAsync(aluguel);

            // Assert - PostgreSQL
            var aluguelInSql = await _context.Alugueis.FindAsync(aluguel.Id);
            aluguelInSql.Should().BeNull();

            // Assert - MongoDB
            var filter = Builders<Aluguel>.Filter.Eq("Id", aluguel.Id);
            var aluguelInMongo = await _mongoCollection.Find(filter).FirstOrDefaultAsync();
            aluguelInMongo.Should().BeNull();
        }
    }
}
