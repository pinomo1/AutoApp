using AutoApp.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace AutoApp.Application.UnitTests.Common;

internal static class TestDbContextFactory
{
    public static AutoDbContext Create()
    {
        var options = new DbContextOptionsBuilder<AutoDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AutoDbContext(options);
    }
}