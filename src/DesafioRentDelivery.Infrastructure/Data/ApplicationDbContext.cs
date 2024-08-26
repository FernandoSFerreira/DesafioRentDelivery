// Infrastructure/Data/ApplicationDbContext.cs
using DesafioRentDelivery.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DesafioRentDelivery.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Moto> Motos { get; set; }
        public DbSet<HistoricoManutencao> HistoricoManutencoes { get; set; }
        public DbSet<Entregador> Entregadores { get; set; }
        public DbSet<Aluguel> Alugueis { get; set; }
        public DbSet<Entrega> Entregas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração da entidade Moto
            modelBuilder.Entity<Moto>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Placa)
                    .IsRequired()
                    .HasMaxLength(10);
                entity.Property(e => e.Modelo)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(e => e.Chassi)
                    .IsRequired()
                    .HasMaxLength(17);

                entity.HasMany(e => e.HistoricoManutencoes)
                    .WithOne(h => h.Moto)
                    .HasForeignKey(h => h.MotoId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<HistoricoManutencao>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.DataManutencao)
                    .IsRequired();
                entity.Property(e => e.Descricao)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            // Configuração da entidade Entregador
            modelBuilder.Entity<Entregador>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(e => e.Documento)
                    .IsRequired()
                    .HasMaxLength(14);
                entity.Property(e => e.Telefone)
                    .IsRequired()
                    .HasMaxLength(15);
            });

            // Configuração da entidade Aluguel
            modelBuilder.Entity<Aluguel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.DataInicio)
                    .IsRequired();
                entity.Property(e => e.DataFim)
                    .IsRequired();

                entity.HasOne(e => e.Entregador)
                    .WithMany(e => e.Alugueis)
                    .HasForeignKey(e => e.EntregadorId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Moto)
                    .WithMany()
                    .HasForeignKey(e => e.MotoId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuração da entidade Entrega
            modelBuilder.Entity<Entrega>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.DataEntrega)
                    .IsRequired();
                entity.Property(e => e.Destino)
                    .IsRequired()
                    .HasMaxLength(255);
                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.HasOne(e => e.Entregador)
                    .WithMany(e => e.Entregas)
                    .HasForeignKey(e => e.EntregadorId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
