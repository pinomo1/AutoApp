using AutoApp.Application.Exceptions;
using AutoApp.Application.Services;
using AutoApp.Application.UnitTests.Common;
using AutoApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutoApp.Application.UnitTests.Services;

public class CountryServiceTests
{
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
        var country = new Country { Id = Guid.NewGuid(), CountryName = "Spain" };
        dbContext.Countries.Add(country);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var service = new CountryService(dbContext);
        await service.DeleteAsync(country.Id, CancellationToken.None);

        var exists = await dbContext.Countries.AnyAsync(c => c.Id == country.Id);
        Assert.That(exists, Is.False);
    }
}