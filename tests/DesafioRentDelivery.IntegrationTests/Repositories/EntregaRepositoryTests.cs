// DesafioRentDelivery.IntegrationTests/Repositories/EntregaRepositoryTests.cs

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
    public class EntregaRepositoryTests : IClassFixture<ApplicationDbContextFixture>
    {
        private readonly EntregaRepository _entregaRepository;
        private readonly ApplicationDbContext _context;
        private readonly IMongoCollection<Entrega> _mongoCollection;

        public EntregaRepositoryTests(ApplicationDbContextFixture fixture)
        {
            _context = fixture.DbContext;
            _mongoCollection = fixture.MongoDatabase.GetCollection<Entrega>("Entregas");

            var loggerMock = new Mock<ILogger<EntregaRepository>>();
            _entregaRepository = new EntregaRepository(_context, fixture.MongoClient, loggerMock.Object);
        }

        [Fact]
        public async Task AddEntregaAsync_ShouldAddEntregaToBothDatabases()
        {
            // Arrange
            var entrega = new Entrega
            {
                EntregadorId = 1,
                DataEntrega = DateTime.Now,
                Destino = "Rua A",
                Status = "Pendente"
            };

            // Act
            await _entregaRepository.AddEntregaAsync(entrega);

            // Assert - PostgreSQL
            var entregaInSql = await _context.Entregas.FindAsync(entrega.Id);
            entregaInSql.Should().NotBeNull();

            // Assert - MongoDB
            var filter = Builders<Entrega>.Filter.Eq("Id", entrega.Id);
            var entregaInMongo = await _mongoCollection.Find(filter).FirstOrDefaultAsync();
            entregaInMongo.Should().NotBeNull();
        }

        [Fact]
        public async Task GetEntregaByIdAsync_ShouldReturnEntrega_WhenEntregaExists()
        {
            // Arrange
            var entrega = _context.Entregas.First();

            // Act
            var result = await _entregaRepository.GetEntregaByIdAsync(entrega.Id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(entrega.Id);
        }

        [Fact]
        public async Task GetAllEntregasAsync_ShouldReturnAllEntregas()
        {
            // Act
            var result = await _entregaRepository.GetAllEntregasAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetEntregasByEntregadorIdAsync_ShouldReturnEntregas_WhenEntregadorHasEntregas()
        {
            // Arrange
            var entregadorId = 1;

            // Act
            var result = await _entregaRepository.GetEntregasByEntregadorIdAsync(entregadorId);

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
        }

        [Fact]
        public async Task UpdateEntregaAsync_ShouldUpdateEntregaInBothDatabases()
        {
            // Arrange
            var entrega = _context.Entregas.First();
            entrega.Destino = "Rua B";

            // Act
            await _entregaRepository.UpdateEntregaAsync(entrega);

            // Assert - PostgreSQL
            var entregaInSql = await _context.Entregas.FindAsync(entrega.Id);
            entregaInSql.Destino.Should().Be(entrega.Destino);

            // Assert - MongoDB
            var filter = Builders<Entrega>.Filter.Eq("Id", entrega.Id);
            var entregaInMongo = await _mongoCollection.Find(filter).FirstOrDefaultAsync();
            entregaInMongo.Destino.Should().Be(entrega.Destino);
        }

        [Fact]
        public async Task RemoveEntregaAsync_ShouldRemoveEntregaFromBothDatabases()
        {
            // Arrange
            var entrega = _context.Entregas.First();

            // Act
            await _entregaRepository.RemoveEntregaAsync(entrega);

            // Assert - PostgreSQL
            var entregaInSql = await _context.Entregas.FindAsync(entrega.Id);
            entregaInSql.Should().BeNull();

            // Assert - MongoDB
            var filter = Builders<Entrega>.Filter.Eq("Id", entrega.Id);
            var entregaInMongo = await _mongoCollection.Find(filter).FirstOrDefaultAsync();
            entregaInMongo.Should().BeNull();
        }
    }
}
