using System.Reflection;
using AutoApp.Domain.Entities;
using AutoApp.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace AutoApp.Infrastructure.Persistence.DbContexts;

public sealed class AutoDbContext(DbContextOptions options) : DbContext(options), IAutoDbContext
{
    public DbSet<Car> Cars { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<Feature> Features { get; set; }
    public DbSet<CarFeature> CarFeature { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new BrandConfiguration());
        modelBuilder.ApplyConfiguration(new CarConfiguration());
        modelBuilder.ApplyConfiguration(new CountryConfiguration());
        modelBuilder.ApplyConfiguration(new FeatureConfiguration());
        modelBuilder.ApplyConfiguration(new CarFeatureConfiguration());
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}