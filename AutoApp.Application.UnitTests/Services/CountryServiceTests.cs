using AutoApp.Application.DTOs.Queries.CountryQueries;
using AutoApp.Application.Exceptions;
using AutoApp.Application.Services;
using AutoApp.Application.UnitTests.Common;
using AutoApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutoApp.Application.UnitTests.Services;

public class CountryServiceTests
{
    [Test]
    public async Task CreateAsync_WhenCalled_ShouldPersistNormalizedCountryCode()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var service = new CountryService(dbContext);

        var id = await service.CreateAsync(new CreateCountryDto("Japan", "jp"), CancellationToken.None);

        var country = await dbContext.Countries.SingleAsync(c => c.Id == id);
        Assert.Multiple(() =>
        {
            Assert.That(country.CountryName, Is.EqualTo("Japan"));
            Assert.That(country.CountryCode, Is.EqualTo("JP"));
        });
    }

    [Test]
    public void GetByIdAsync_WhenCountryDoesNotExist_ShouldThrowNotFoundException()
    {
        using var dbContext = TestDbContextFactory.Create();
        var service = new CountryService(dbContext);

        Assert.ThrowsAsync<NotFoundException>(() => service.GetByIdAsync(Guid.NewGuid(), CancellationToken.None));
    }

    [Test]
    public async Task DeleteAsync_WhenCountryExists_ShouldRemoveCountry()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var country = new Country { Id = Guid.NewGuid(), CountryName = "Spain", CountryCode = "ES" };
        dbContext.Countries.Add(country);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var service = new CountryService(dbContext);
        await service.DeleteAsync(country.Id, CancellationToken.None);

        var exists = await dbContext.Countries.AnyAsync(c => c.Id == country.Id);
        Assert.That(exists, Is.False);
    }
}