using AutoApp.Application.DTOs.Queries.BrandQueries;
using AutoApp.Application.Exceptions;
using AutoApp.Application.Services;
using AutoApp.Application.UnitTests.Common;
using AutoApp.Domain.Entities;
using AutoApp.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace AutoApp.Application.UnitTests.Services;

public class BrandServiceTests
{
    private static readonly IBrandLogoStorage StubStorage = new StubBrandLogoStorage("https://cdn.example.com/default-logo.png");

    [Test]
    public async Task CreateAsync_WhenCountryExists_ShouldCreateBrandWithCountry()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var country = new Country { Id = Guid.NewGuid(), CountryName = "Germany", CountryCode = "DE" };
        dbContext.Countries.Add(country);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var service = new BrandService(dbContext, StubStorage);
        var id = await service.CreateAsync(new CreateBrandDto("  BMW  ", country.Id, "  https://cdn.example.com/bmw.png  "), CancellationToken.None);

        var brand = await dbContext.Brands.SingleAsync(b => b.Id == id);
        Assert.Multiple(() =>
        {
            Assert.That(brand.BrandName, Is.EqualTo("BMW"));
            Assert.That(brand.CountryId, Is.EqualTo(country.Id));
            Assert.That(brand.LogoUrl, Is.EqualTo("https://cdn.example.com/bmw.png"));
        });
    }

    [Test]
    public void CreateAsync_WhenCountryDoesNotExist_ShouldThrowNotFoundException()
    {
        using var dbContext = TestDbContextFactory.Create();
        var service = new BrandService(dbContext, StubStorage);

        Assert.ThrowsAsync<NotFoundException>(() =>
            service.CreateAsync(new CreateBrandDto("Audi", Guid.NewGuid()), CancellationToken.None));
    }

    [Test]
    public async Task GetByIdAsync_WhenBrandExists_ShouldReturnMappedDto()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var country = new Country { Id = Guid.NewGuid(), CountryName = "Japan", CountryCode = "JP" };
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

        var service = new BrandService(dbContext, StubStorage);
        var dto = await service.GetByIdAsync(brand.Id, CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(dto.Id, Is.EqualTo(brand.Id));
            Assert.That(dto.BrandName, Is.EqualTo("Toyota"));
            Assert.That(dto.CountryId, Is.EqualTo(country.Id));
            Assert.That(dto.CountryName, Is.EqualTo("Japan"));
            Assert.That(dto.LogoUrl, Is.Null);
        });
    }

    [Test]
    public void GetByIdAsync_WhenBrandDoesNotExist_ShouldThrowNotFoundException()
    {
        using var dbContext = TestDbContextFactory.Create();
        var service = new BrandService(dbContext, StubStorage);

        Assert.ThrowsAsync<NotFoundException>(() => service.GetByIdAsync(Guid.NewGuid(), CancellationToken.None));
    }

    [Test]
    public async Task UploadLogoAsync_WhenBrandExists_ShouldPersistReturnedLogoUrl()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var country = new Country { Id = Guid.NewGuid(), CountryName = "Germany", CountryCode = "DE" };
        var brand = new Brand { Id = Guid.NewGuid(), BrandName = "BMW", CountryId = country.Id, Country = country };
        dbContext.Countries.Add(country);
        dbContext.Brands.Add(brand);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        const string storedUrl = "https://cdn.example.com/brand-logo.png";
        var service = new BrandService(dbContext, new StubBrandLogoStorage(storedUrl));
        await using var content = new MemoryStream([1, 2, 3, 4]);

        var dto = await service.UploadLogoAsync(brand.Id, content, "bmw.png", CancellationToken.None);

        var updated = await dbContext.Brands.SingleAsync(b => b.Id == brand.Id);
        Assert.Multiple(() =>
        {
            Assert.That(updated.LogoUrl, Is.EqualTo(storedUrl));
            Assert.That(dto.LogoUrl, Is.EqualTo(storedUrl));
        });
    }

    [Test]
    public void UploadLogoAsync_WhenBrandMissing_ShouldThrowNotFoundException()
    {
        using var dbContext = TestDbContextFactory.Create();
        var service = new BrandService(dbContext, StubStorage);
        using var content = new MemoryStream([1, 2]);

        Assert.ThrowsAsync<NotFoundException>(() =>
            service.UploadLogoAsync(Guid.NewGuid(), content, "unknown.png", CancellationToken.None));
    }

    private sealed class StubBrandLogoStorage(string logoUrl) : IBrandLogoStorage
    {
        public Task<string> UploadAsync(Guid brandId, Stream content, string originalFileName, CancellationToken ct)
            => Task.FromResult(logoUrl);
    }
}