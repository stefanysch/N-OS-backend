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
    public DbSet<Peca> Pecas => Set<Peca>();
}