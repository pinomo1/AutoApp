using AutoApp.Application.DTOs.Queries.CarQueries;
using AutoApp.Application.Exceptions;
using AutoApp.Application.Services;
using AutoApp.Application.UnitTests.Common;
using AutoApp.Domain.Entities;
using AutoApp.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AutoApp.Application.UnitTests.Services;

public class CarServiceTests
{
    [Test]
    public void CreateAsync_WhenBrandDoesNotExist_ShouldThrowNotFoundException()
    {
        using var dbContext = TestDbContextFactory.Create();
        var service = new CarService(dbContext);

        Assert.ThrowsAsync<NotFoundException>(() => service.CreateAsync(CreateCarDto(Guid.NewGuid()), CancellationToken.None));
    }

    [Test]
    public async Task GetByIdAsync_WhenCarExists_ShouldReturnMappedDto()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var country = new Country { Id = Guid.NewGuid(), CountryName = "Japan", CountryCode = "JP" };
        var brand = new Brand { Id = Guid.NewGuid(), BrandName = "Toyota", CountryId = country.Id, Country = country };
        var feature = new Feature { Id = Guid.NewGuid(), FeatureName = "Sunroof" };
        var car = new Car
        {
            Id = Guid.NewGuid(),
            BrandId = brand.Id,
            Brand = brand,
            Model = "Camry",
            Year = 2020,
            CarCondition = CarCondition.Used,
            CarType = CarType.Sedan,
            FuelType = FuelType.Gasoline,
            TransmissionType = TransmissionType.Automatic,
            Color = Color.Black,
            Horsepower = 180,
            EngineVolumeCc = 2500,
            Price = 20000m,
            Mileage = 50000,
            Features = new List<Feature> { feature }
        };

        dbContext.Countries.Add(country);
        dbContext.Brands.Add(brand);
        dbContext.Features.Add(feature);
        dbContext.Cars.Add(car);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var service = new CarService(dbContext);
        var dto = await service.GetByIdAsync(car.Id, CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(dto.Id, Is.EqualTo(car.Id));
            Assert.That(dto.BrandId, Is.EqualTo(brand.Id));
            Assert.That(dto.BrandName, Is.EqualTo("Toyota"));
            Assert.That(dto.Model, Is.EqualTo("Camry"));
            Assert.That(dto.Features, Has.Count.EqualTo(1));
            Assert.That(dto.Features[0].FeatureName, Is.EqualTo("Sunroof"));
        });
    }

    [Test]
    public void GetByIdAsync_WhenCarDoesNotExist_ShouldThrowNotFoundException()
    {
        using var dbContext = TestDbContextFactory.Create();
        var service = new CarService(dbContext);

        Assert.ThrowsAsync<NotFoundException>(() => service.GetByIdAsync(Guid.NewGuid(), CancellationToken.None));
    }

    [Test]
    public async Task AddFeatureAsync_WhenRelationDoesNotExist_ShouldCreateRelation()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var brand = new Brand { Id = Guid.NewGuid(), BrandName = "Toyota", CountryId = Guid.NewGuid() };
        var car = new Car
        {
            Id = Guid.NewGuid(),
            BrandId = brand.Id,
            Model = "Camry",
            Year = 2020,
            CarCondition = CarCondition.Used,
            CarType = CarType.Sedan,
            FuelType = FuelType.Gasoline,
            TransmissionType = TransmissionType.Automatic,
            Color = Color.Black,
            Horsepower = 180,
            EngineVolumeCc = 2500,
            Price = 20000m,
            Mileage = 50000
        };
        var feature = new Feature { Id = Guid.NewGuid(), FeatureName = "Sunroof" };

        dbContext.Brands.Add(brand);
        dbContext.Cars.Add(car);
        dbContext.Features.Add(feature);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var service = new CarService(dbContext);
        await service.AddFeatureAsync(car.Id, feature.Id, CancellationToken.None);

        var relationExists = await dbContext.CarFeature.AnyAsync(cf => cf.CarId == car.Id && cf.FeatureId == feature.Id);
        Assert.That(relationExists, Is.True);
    }

    [Test]
    public async Task AddFeatureAsync_WhenRelationAlreadyExists_ShouldNotDuplicateRelation()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var brand = new Brand { Id = Guid.NewGuid(), BrandName = "Toyota", CountryId = Guid.NewGuid() };
        var car = new Car
        {
            Id = Guid.NewGuid(),
            BrandId = brand.Id,
            Model = "Corolla",
            Year = 2019,
            CarCondition = CarCondition.Used,
            CarType = CarType.Sedan,
            FuelType = FuelType.Gasoline,
            TransmissionType = TransmissionType.Automatic,
            Color = Color.White,
            Horsepower = 132,
            EngineVolumeCc = 1800,
            Price = 17000m,
            Mileage = 60000
        };
        var feature = new Feature { Id = Guid.NewGuid(), FeatureName = "Cruise Control" };

        dbContext.Brands.Add(brand);
        dbContext.Cars.Add(car);
        dbContext.Features.Add(feature);
        dbContext.CarFeature.Add(new CarFeature { CarId = car.Id, FeatureId = feature.Id });
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var service = new CarService(dbContext);
        await service.AddFeatureAsync(car.Id, feature.Id, CancellationToken.None);

        var relationCount = await dbContext.CarFeature.CountAsync(cf => cf.CarId == car.Id && cf.FeatureId == feature.Id);
        Assert.That(relationCount, Is.EqualTo(1));
    }

    [Test]
    public async Task RemoveFeatureAsync_WhenRelationExists_ShouldRemoveRelation()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var brand = new Brand { Id = Guid.NewGuid(), BrandName = "Toyota", CountryId = Guid.NewGuid() };
        var car = new Car
        {
            Id = Guid.NewGuid(),
            BrandId = brand.Id,
            Model = "Rav4",
            Year = 2022,
            CarCondition = CarCondition.Used,
            CarType = CarType.Suv,
            FuelType = FuelType.Hybrid,
            TransmissionType = TransmissionType.Automatic,
            Color = Color.Blue,
            Horsepower = 219,
            EngineVolumeCc = 2500,
            Price = 32000m,
            Mileage = 18000
        };
        var feature = new Feature { Id = Guid.NewGuid(), FeatureName = "Heated Seats" };

        dbContext.Brands.Add(brand);
        dbContext.Cars.Add(car);
        dbContext.Features.Add(feature);
        dbContext.CarFeature.Add(new CarFeature { CarId = car.Id, FeatureId = feature.Id });
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var service = new CarService(dbContext);
        await service.RemoveFeatureAsync(car.Id, feature.Id, CancellationToken.None);

        var relationExists = await dbContext.CarFeature.AnyAsync(cf => cf.CarId == car.Id && cf.FeatureId == feature.Id);
        Assert.That(relationExists, Is.False);
    }

    [Test]
    public async Task RemoveFeatureAsync_WhenRelationDoesNotExist_ShouldThrowNotFoundException()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var brand = new Brand { Id = Guid.NewGuid(), BrandName = "Toyota", CountryId = Guid.NewGuid() };
        var car = new Car
        {
            Id = Guid.NewGuid(),
            BrandId = brand.Id,
            Model = "Prius",
            Year = 2021,
            CarCondition = CarCondition.Used,
            CarType = CarType.Hatchback,
            FuelType = FuelType.Hybrid,
            TransmissionType = TransmissionType.Automatic,
            Color = Color.Silver,
            Horsepower = 121,
            EngineVolumeCc = 1800,
            Price = 26000m,
            Mileage = 28000
        };
        var feature = new Feature { Id = Guid.NewGuid(), FeatureName = "Apple CarPlay" };

        dbContext.Brands.Add(brand);
        dbContext.Cars.Add(car);
        dbContext.Features.Add(feature);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var service = new CarService(dbContext);

        Assert.ThrowsAsync<NotFoundException>(() => service.RemoveFeatureAsync(car.Id, feature.Id, CancellationToken.None));
    }

    private static CreateCarDto CreateCarDto(Guid brandId)
    {
        return new CreateCarDto(
            brandId,
            "  Model S  ",
            2024,
            CarCondition.New,
            CarType.Sedan,
            FuelType.Electric,
            TransmissionType.Automatic,
            Color.White,
            670,
            1,
            85000m,
            0);
    }
}