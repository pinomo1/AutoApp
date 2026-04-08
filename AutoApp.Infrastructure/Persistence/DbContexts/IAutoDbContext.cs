using AutoApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutoApp.Infrastructure.Persistence.DbContexts;

public interface IAutoDbContext
{
    public DbSet<Car> Cars { get; set; }
    Task<int> SaveChangesAsync(CancellationToken ct);
}