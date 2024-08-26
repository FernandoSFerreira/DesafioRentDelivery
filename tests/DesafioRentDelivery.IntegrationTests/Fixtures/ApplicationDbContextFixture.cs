// DesafioRentDelivery.IntegrationTests/Fixtures/ApplicationDbContextFixture.cs

using DesafioRentDelivery.Domain.Entities;
using DesafioRentDelivery.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using System;

namespace DesafioRentDelivery.IntegrationTests.Fixtures
{
    public class ApplicationDbContextFixture : IDisposable
    {
        public ApplicationDbContext DbContext { get; private set; }
        public IMongoClient MongoClient { get; private set; }
        public IMongoDatabase MongoDatabase { get; private set; }

        public ApplicationDbContextFixture()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            DbContext = new ApplicationDbContext(options);

            MongoClient = new MongoClient("mongodb://localhost:27017");
            MongoDatabase = MongoClient.GetDatabase("TestDatabase");

            // Seed the database with some data
            SeedDatabase();
        }

        private void SeedDatabase()
        {
            DbContext.Entregadores.AddRange(
                new Entregador { Nome = "Entregador 1", Documento = "11111111111", Telefone = "11911111111" },
                new Entregador { Nome = "Entregador 2", Documento = "22222222222", Telefone = "11922222222" }
            );

            DbContext.SaveChanges();
        }

        public void Dispose()
        {
            DbContext.Dispose();
            MongoClient = null;
            MongoDatabase = null;
        }
    }
}
