using AutoApp.Application.DTOs.Queries.BrandQueries;
using AutoApp.Application.Exceptions;
using AutoApp.Application.Services;
using AutoApp.Application.UnitTests.Common;
using AutoApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutoApp.Application.UnitTests.Services;

public class BrandServiceTests
{
    [Test]
    public async Task CreateAsync_WhenCountryExists_ShouldCreateBrandWithCountry()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var country = new Country { Id = Guid.NewGuid(), CountryName = "Germany" };
        dbContext.Countries.Add(country);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var service = new BrandService(dbContext);
        var id = await service.CreateAsync(new CreateBrandDto("  BMW  ", country.Id), CancellationToken.None);

        var brand = await dbContext.Brands.SingleAsync(b => b.Id == id);
        Assert.Multiple(() =>
        {
            Assert.That(brand.BrandName, Is.EqualTo("BMW"));
            Assert.That(brand.CountryId, Is.EqualTo(country.Id));
        });
    }

    [Test]
    public void CreateAsync_WhenCountryDoesNotExist_ShouldThrowNotFoundException()
    {
        using var dbContext = TestDbContextFactory.Create();
        var service = new BrandService(dbContext);

        Assert.ThrowsAsync<NotFoundException>(() =>
            service.CreateAsync(new CreateBrandDto("Audi", Guid.NewGuid()), CancellationToken.None));
    }

    [Test]
    public async Task GetByIdAsync_WhenBrandExists_ShouldReturnMappedDto()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var country = new Country { Id = Guid.NewGuid(), CountryName = "Japan" };
        var brand = new Brand
        {
            Id = Guid.NewGuid(),
            BrandName = "Toyota",
            CountryId = country.Id,
            Country = country
        };

        dbContext.Countries.Add(country);
        dbContext.Brands.Add(brand);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var service = new BrandService(dbContext);
        var dto = await service.GetByIdAsync(brand.Id, CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(dto.Id, Is.EqualTo(brand.Id));
            Assert.That(dto.BrandName, Is.EqualTo("Toyota"));
            Assert.That(dto.CountryId, Is.EqualTo(country.Id));
            Assert.That(dto.CountryName, Is.EqualTo("Japan"));
        });
    }

    [Test]
    public void GetByIdAsync_WhenBrandDoesNotExist_ShouldThrowNotFoundException()
    {
        using var dbContext = TestDbContextFactory.Create();
        var service = new BrandService(dbContext);

        Assert.ThrowsAsync<NotFoundException>(() => service.GetByIdAsync(Guid.NewGuid(), CancellationToken.None));
    }
}