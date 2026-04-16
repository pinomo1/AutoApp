using AutoApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutoApp.Infrastructure.Persistence.DbContexts;

public interface IAutoDbContext
{
    public DbSet<Car> Cars { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<Feature> Features { get; set; }
    public DbSet<CarFeature> CarFeature { get; set; }
    Task<int> SaveChangesAsync(CancellationToken ct);
}