using Microsoft.EntityFrameworkCore;
using N_OS.Domain.Entities;

namespace N_OS.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Veiculo> Veiculos => Set<Veiculo>();
    public DbSet<Peca> Pecas => Set<Peca>();
    public DbSet<Servico> Servicos => Set<Servico>();
    public DbSet<OrdemDeServico> OrdensDeServico { get; set; }
    public DbSet<ItemOS> ItensOS { get; set; }
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Cliente>(cliente =>
        {
            cliente.OwnsOne(c => c.Documento, documento =>
            {
                documento.Property(d => d.Tipo)
                    .HasColumnName("TipoDocumento");

                documento.Property(d => d.Numero)
                    .HasColumnName("Documento")
                    .HasMaxLength(14);

                documento.HasIndex(d => d.Numero)
                    .IsUnique();
            });

            cliente.OwnsOne(c => c.Endereco, endereco =>
            {
                endereco.ToTable("ClienteEnderecos");

                endereco.WithOwner().HasForeignKey("ClienteId");

                endereco.Property(e => e.Cep)
                    .HasColumnName("Cep")
                    .HasMaxLength(9);

                endereco.Property(e => e.Logradouro)
                    .HasColumnName("Logradouro")
                    .HasMaxLength(150);

                endereco.Property(e => e.Numero)
                    .HasColumnName("Numero")
                    .HasMaxLength(20);

                endereco.Property(e => e.Complemento)
                    .HasColumnName("Complemento")
                    .HasMaxLength(100);

                endereco.Property(e => e.Bairro)
                    .HasColumnName("Bairro")
                    .HasMaxLength(100);

                endereco.Property(e => e.Cidade)
                    .HasColumnName("Cidade")
                    .HasMaxLength(100);

                endereco.Property(e => e.Estado)
                    .HasColumnName("Estado")
                    .HasMaxLength(2);
            });

            cliente.HasMany(c => c.Veiculos)
                .WithOne(v => v.Cliente)
                .HasForeignKey(v => v.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Usuario>(usuario =>
        {
            usuario.HasIndex(u => u.Email)
                .IsUnique();
        });
    }
}