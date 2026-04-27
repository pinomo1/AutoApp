using AutoApp.Domain.Entities;
using AutoApp.Infrastructure.Persistence.DbContexts;
using AutoApp.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace AutoApp.Infrastructure.UnitTests.Services;

public class AutoSaveChangesInterceptorTests
{
    [Test]
    public void SaveChanges_WhenEntityIsAdded_ShouldSetCreatedAt()
    {
        using var dbContext = CreateDbContext(new AutoSaveChangesInterceptor());
        var country = new Country { CountryName = "Japan", CountryCode = "JP" };

        dbContext.Countries.Add(country);
        dbContext.SaveChanges();

        Assert.Multiple(() =>
        {
            Assert.That(country.CreatedAt, Is.Not.EqualTo(default(DateTime)));
            Assert.That(country.UpdatedAt, Is.Not.EqualTo(default(DateTime)));
            Assert.That(country.UpdatedAt, Is.EqualTo(country.CreatedAt));
        });
    }

    [Test]
    public void SaveChanges_WhenEntityIsModified_ShouldSetUpdatedAt()
    {
        using var dbContext = CreateDbContext(new AutoSaveChangesInterceptor());
        var country = new Country { CountryName = "Germany", CountryCode = "DE" };

        dbContext.Countries.Add(country);
        dbContext.SaveChanges();

        var createdAtOnInsert = country.CreatedAt;
        var updatedAtOnInsert = country.UpdatedAt;

        country.CountryName = "France";
        dbContext.SaveChanges();

        Assert.Multiple(() =>
        {
            Assert.That(country.CreatedAt, Is.Not.EqualTo(default(DateTime)));
            Assert.That(country.UpdatedAt, Is.Not.EqualTo(default(DateTime)));
            Assert.That(country.CreatedAt, Is.EqualTo(createdAtOnInsert));
            Assert.That(country.UpdatedAt, Is.GreaterThanOrEqualTo(updatedAtOnInsert));
        });
    }

    private static AutoDbContext CreateDbContext(AutoSaveChangesInterceptor interceptor)
    {
        var options = new DbContextOptionsBuilder<AutoDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .AddInterceptors(interceptor)
            .Options;

        return new AutoDbContext(options);
    }
}