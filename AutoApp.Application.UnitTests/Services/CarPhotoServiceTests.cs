using AutoApp.Application.Exceptions;
using AutoApp.Application.Services;
using AutoApp.Application.UnitTests.Common;
using AutoApp.Domain.Entities;
using AutoApp.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace AutoApp.Application.UnitTests.Services;

public class CarPhotoServiceTests
{
    private static readonly ICarPhotoStorage StubStorage = new StubCarPhotoStorage("https://cdn.example.com/car-photos/photo.jpg");

    [Test]
    public async Task GetByCarIdAsync_WhenCarExists_ShouldReturnPhotos()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var country = new Country { Id = Guid.NewGuid(), CountryName = "USA", CountryCode = "US" };
        var brand = new Brand { Id = Guid.NewGuid(), BrandName = "Tesla", CountryId = country.Id, Country = country };
        var car = new Car { Id = Guid.NewGuid(), BrandId = brand.Id, Brand = brand, Model = "Model S", Year = 2024 };
        var photo1 = new CarPhoto { Id = Guid.NewGuid(), CarId = car.Id, Car = car, PhotoUrl = "http://example.com/photo1.jpg", DisplayOrder = 0, IsMainPhoto = true };
        var photo2 = new CarPhoto { Id = Guid.NewGuid(), CarId = car.Id, Car = car, PhotoUrl = "http://example.com/photo2.jpg", DisplayOrder = 1, IsMainPhoto = false };

        dbContext.Countries.Add(country);
        dbContext.Brands.Add(brand);
        dbContext.Cars.Add(car);
        dbContext.CarPhotos.Add(photo1);
        dbContext.CarPhotos.Add(photo2);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var service = new CarPhotoService(dbContext, StubStorage);
        var photos = await service.GetByCarIdAsync(car.Id, CancellationToken.None);

        Assert.That(photos, Has.Count.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(photos[0].IsMainPhoto, Is.True);
            Assert.That(photos[1].IsMainPhoto, Is.False);
        }
    }

    [Test]
    public void GetByCarIdAsync_WhenCarDoesNotExist_ShouldThrowNotFoundException()
    {
        using var dbContext = TestDbContextFactory.Create();
        var service = new CarPhotoService(dbContext, StubStorage);

        Assert.ThrowsAsync<NotFoundException>(() => service.GetByCarIdAsync(Guid.NewGuid(), CancellationToken.None));
    }

    [Test]
    public async Task GetByIdAsync_WhenPhotoExists_ShouldReturnMappedDto()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var country = new Country { Id = Guid.NewGuid(), CountryName = "USA", CountryCode = "US" };
        var brand = new Brand { Id = Guid.NewGuid(), BrandName = "Tesla", CountryId = country.Id, Country = country };
        var car = new Car { Id = Guid.NewGuid(), BrandId = brand.Id, Brand = brand, Model = "Model S", Year = 2024 };
        var photo = new CarPhoto { Id = Guid.NewGuid(), CarId = car.Id, Car = car, PhotoUrl = "http://example.com/photo.jpg", DisplayOrder = 0, IsMainPhoto = true };

        dbContext.Countries.Add(country);
        dbContext.Brands.Add(brand);
        dbContext.Cars.Add(car);
        dbContext.CarPhotos.Add(photo);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var service = new CarPhotoService(dbContext, StubStorage);
        var dto = await service.GetByIdAsync(photo.Id, CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(dto.Id, Is.EqualTo(photo.Id));
            Assert.That(dto.CarId, Is.EqualTo(car.Id));
            Assert.That(dto.PhotoUrl, Is.EqualTo("http://example.com/photo.jpg"));
            Assert.That(dto.IsMainPhoto, Is.True);
        });
    }

    [Test]
    public void GetByIdAsync_WhenPhotoDoesNotExist_ShouldThrowNotFoundException()
    {
        using var dbContext = TestDbContextFactory.Create();
        var service = new CarPhotoService(dbContext, StubStorage);

        Assert.ThrowsAsync<NotFoundException>(() => service.GetByIdAsync(Guid.NewGuid(), CancellationToken.None));
    }

    [Test]
    public async Task CreateAsync_WhenCarExists_ShouldCreatePhotoAndUnsetPreviousMainPhoto()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var country = new Country { Id = Guid.NewGuid(), CountryName = "USA", CountryCode = "US" };
        var brand = new Brand { Id = Guid.NewGuid(), BrandName = "Tesla", CountryId = country.Id, Country = country };
        var car = new Car { Id = Guid.NewGuid(), BrandId = brand.Id, Brand = brand, Model = "Model S", Year = 2024 };
        var existingMainPhoto = new CarPhoto { Id = Guid.NewGuid(), CarId = car.Id, Car = car, PhotoUrl = "http://example.com/old-main.jpg", DisplayOrder = 0, IsMainPhoto = true };

        dbContext.Countries.Add(country);
        dbContext.Brands.Add(brand);
        dbContext.Cars.Add(car);
        dbContext.CarPhotos.Add(existingMainPhoto);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var service = new CarPhotoService(dbContext, StubStorage);
        await using var content = new MemoryStream([1, 2, 3]);
        var photoId = await service.CreateAsync(car.Id, content, "photo.jpg", 1, true, CancellationToken.None);

        var createdPhoto = await dbContext.CarPhotos.FirstOrDefaultAsync(p => p.Id == photoId);
        Assert.That(createdPhoto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(createdPhoto!.PhotoUrl, Is.EqualTo("https://cdn.example.com/car-photos/photo.jpg"));
            Assert.That(createdPhoto.DisplayOrder, Is.EqualTo(1));
            Assert.That(createdPhoto.IsMainPhoto, Is.True);
            Assert.That(existingMainPhoto.IsMainPhoto, Is.False);
        });
    }

    [Test]
    public void CreateAsync_WhenCarDoesNotExist_ShouldThrowNotFoundException()
    {
        using var dbContext = TestDbContextFactory.Create();
        var service = new CarPhotoService(dbContext, StubStorage);
        using var content = new MemoryStream([1, 2]);

        Assert.ThrowsAsync<NotFoundException>(() => service.CreateAsync(Guid.NewGuid(), content, "photo.jpg", 0, true, CancellationToken.None));
    }

    [Test]
    public async Task UpdateAsync_WhenPhotoExists_ShouldUpdatePhotoAndUnsetOtherMainPhotos()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var country = new Country { Id = Guid.NewGuid(), CountryName = "USA", CountryCode = "US" };
        var brand = new Brand { Id = Guid.NewGuid(), BrandName = "Tesla", CountryId = country.Id, Country = country };
        var car = new Car { Id = Guid.NewGuid(), BrandId = brand.Id, Brand = brand, Model = "Model S", Year = 2024 };
        var photo = new CarPhoto { Id = Guid.NewGuid(), CarId = car.Id, Car = car, PhotoUrl = "http://example.com/photo.jpg", DisplayOrder = 0, IsMainPhoto = false };
        var otherMainPhoto = new CarPhoto { Id = Guid.NewGuid(), CarId = car.Id, Car = car, PhotoUrl = "http://example.com/main.jpg", DisplayOrder = 1, IsMainPhoto = true };

        dbContext.Countries.Add(country);
        dbContext.Brands.Add(brand);
        dbContext.Cars.Add(car);
        dbContext.CarPhotos.Add(photo);
        dbContext.CarPhotos.Add(otherMainPhoto);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var service = new CarPhotoService(dbContext, StubStorage);
        await using var content = new MemoryStream([4, 5, 6]);
        await service.UpdateAsync(car.Id, photo.Id, content, "updated.jpg", 2, true, CancellationToken.None);

        var updatedPhoto = await dbContext.CarPhotos.FirstOrDefaultAsync(p => p.Id == photo.Id);
        Assert.That(updatedPhoto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(updatedPhoto!.PhotoUrl, Is.EqualTo("https://cdn.example.com/car-photos/photo.jpg"));
            Assert.That(updatedPhoto.DisplayOrder, Is.EqualTo(2));
            Assert.That(updatedPhoto.IsMainPhoto, Is.True);
            Assert.That(otherMainPhoto.IsMainPhoto, Is.False);
        });
    }

    [Test]
    public void UpdateAsync_WhenPhotoDoesNotExist_ShouldThrowNotFoundException()
    {
        using var dbContext = TestDbContextFactory.Create();
        var service = new CarPhotoService(dbContext, StubStorage);
        using var content = new MemoryStream([1, 2]);

        Assert.ThrowsAsync<NotFoundException>(() => service.UpdateAsync(Guid.NewGuid(), Guid.NewGuid(), content, "photo.jpg", 0, true, CancellationToken.None));
    }

    [Test]
    public async Task DeleteAsync_WhenPhotoExists_ShouldRemovePhoto()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var country = new Country { Id = Guid.NewGuid(), CountryName = "USA", CountryCode = "US" };
        var brand = new Brand { Id = Guid.NewGuid(), BrandName = "Tesla", CountryId = country.Id, Country = country };
        var car = new Car { Id = Guid.NewGuid(), BrandId = brand.Id, Brand = brand, Model = "Model S", Year = 2024 };
        var photo = new CarPhoto { Id = Guid.NewGuid(), CarId = car.Id, Car = car, PhotoUrl = "http://example.com/photo.jpg", DisplayOrder = 0 };

        dbContext.Countries.Add(country);
        dbContext.Brands.Add(brand);
        dbContext.Cars.Add(car);
        dbContext.CarPhotos.Add(photo);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var service = new CarPhotoService(dbContext, StubStorage);
        await service.DeleteAsync(photo.Id, CancellationToken.None);

        var exists = await dbContext.CarPhotos.AnyAsync(p => p.Id == photo.Id);
        Assert.That(exists, Is.False);
    }

    [Test]
    public void DeleteAsync_WhenPhotoDoesNotExist_ShouldThrowNotFoundException()
    {
        using var dbContext = TestDbContextFactory.Create();
        var service = new CarPhotoService(dbContext, StubStorage);

        Assert.ThrowsAsync<NotFoundException>(() => service.DeleteAsync(Guid.NewGuid(), CancellationToken.None));
    }

    [Test]
    public async Task DeleteByCarIdAsync_WhenCarExists_ShouldRemoveAllPhotos()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var country = new Country { Id = Guid.NewGuid(), CountryName = "USA", CountryCode = "US" };
        var brand = new Brand { Id = Guid.NewGuid(), BrandName = "Tesla", CountryId = country.Id, Country = country };
        var car = new Car { Id = Guid.NewGuid(), BrandId = brand.Id, Brand = brand, Model = "Model S", Year = 2024 };
        var photo1 = new CarPhoto { Id = Guid.NewGuid(), CarId = car.Id, Car = car, PhotoUrl = "http://example.com/photo1.jpg", DisplayOrder = 0 };
        var photo2 = new CarPhoto { Id = Guid.NewGuid(), CarId = car.Id, Car = car, PhotoUrl = "http://example.com/photo2.jpg", DisplayOrder = 1 };

        dbContext.Countries.Add(country);
        dbContext.Brands.Add(brand);
        dbContext.Cars.Add(car);
        dbContext.CarPhotos.Add(photo1);
        dbContext.CarPhotos.Add(photo2);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var service = new CarPhotoService(dbContext, StubStorage);
        await service.DeleteByCarIdAsync(car.Id, CancellationToken.None);

        var photoCount = await dbContext.CarPhotos.CountAsync(p => p.CarId == car.Id);
        Assert.That(photoCount, Is.EqualTo(0));
    }

    private sealed class StubCarPhotoStorage(string photoUrl) : ICarPhotoStorage
    {
        public Task<string> UploadAsync(Guid carPhotoId, Stream content, string originalFileName, CancellationToken ct)
            => Task.FromResult(photoUrl);
    }
}
