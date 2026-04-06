using System.Reflection;
using AutoApp.Domain.Entities;
using AutoApp.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace AutoApp.Infrastructure.Persistence.DbContexts;

public sealed class AutoDbContext : DbContext, IAutoDbContext
{
    public AutoDbContext(DbContextOptions options) : base(options)
    {
        Database.EnsureCreated();
    }
    
    public DbSet<Car> Cars { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new CarConfiguration());
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}