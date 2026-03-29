using Microsoft.EntityFrameworkCore;
using SuaApi.Models;

namespace SuaApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Equipamento> Equipamentos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Equipamento>(entity =>
        {
            // Força o nome da tabela em minúsculo
            entity.ToTable("equipamentos");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Codigo).HasColumnName("codigo");
            
            // 1. Configura o Enum Tipo para ser lido/gravado como Texto
            entity.Property(e => e.Tipo)
                .HasColumnName("tipo")
                .HasConversion<string>(); 

            // 2. Configura o Enum StatusOperacional para ser lido/gravado como Texto
            entity.Property(e => e.StatusOperacional)
                .HasColumnName("statusoperacional")
                .HasConversion<string>();

            entity.Property(e => e.Modelo).HasColumnName("modelo");
            entity.Property(e => e.Horimetro).HasColumnName("horimetro");
            entity.Property(e => e.DataAquisicao).HasColumnName("dataaquisicao");
            entity.Property(e => e.LocalizacaoAtual).HasColumnName("localizacaoatual");
        });

        modelBuilder.Entity<Equipamento>()
            .HasIndex(e => e.Codigo)
            .IsUnique();
    }
}