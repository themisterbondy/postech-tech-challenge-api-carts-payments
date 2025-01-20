using Microsoft.EntityFrameworkCore;
using Postech.Fiap.CartsPayments.WebApi.Features.Carts.Entities;
using Postech.Fiap.CartsPayments.WebApi.Features.Orders.Entities;

namespace Postech.Fiap.CartsPayments.WebApi.Persistence;

[ExcludeFromCodeCoverage]
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("uuid-ossp");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}