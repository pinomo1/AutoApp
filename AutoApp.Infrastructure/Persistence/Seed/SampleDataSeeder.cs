using AutoApp.Domain.Entities;
using AutoApp.Domain.Enums;
using AutoApp.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AutoApp.Infrastructure.Persistence.Seed;

public static class SampleDataSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider, CancellationToken ct = default)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AutoDbContext>();

        if (await context.Countries.AnyAsync(ct) ||
            await context.Brands.AnyAsync(ct) ||
            await context.Features.AnyAsync(ct) ||
            await context.Cars.AnyAsync(ct))
        {
            return;
        }

        var japan = new Country
        {
            Id = Guid.NewGuid(),
            CountryName = "Japan",
            CountryCode = "JP"
        };

        var germany = new Country
        {
            Id = Guid.NewGuid(),
            CountryName = "Germany",
            CountryCode = "DE"
        };

        var toyota = new Brand
        {
            Id = Guid.NewGuid(),
            BrandName = "Toyota",
            CountryId = japan.Id
        };

        var bmw = new Brand
        {
            Id = Guid.NewGuid(),
            BrandName = "BMW",
            CountryId = germany.Id
        };

        var sunroof = new Feature
        {
            Id = Guid.NewGuid(),
            FeatureName = "Sunroof"
        };

        var heatedSeats = new Feature
        {
            Id = Guid.NewGuid(),
            FeatureName = "Heated Seats"
        };

        var toyotaCamry = new Car
        {
            Id = Guid.NewGuid(),
            BrandId = toyota.Id,
            Model = "Camry",
            Year = 2024,
            CarCondition = CarCondition.New,
            CarType = CarType.Sedan,
            FuelType = FuelType.Hybrid,
            TransmissionType = TransmissionType.Automatic,
            Color = Color.Black,
            Horsepower = 208,
            EngineVolumeCc = 2500,
            Price = 32000m,
            Mileage = 0
        };

        var bmwX5 = new Car
        {
            Id = Guid.NewGuid(),
            BrandId = bmw.Id,
            Model = "X5",
            Year = 2023,
            CarCondition = CarCondition.Used,
            CarType = CarType.Suv,
            FuelType = FuelType.Gasoline,
            TransmissionType = TransmissionType.Automatic,
            Color = Color.White,
            Horsepower = 335,
            EngineVolumeCc = 3000,
            Price = 58900m,
            Mileage = 15000
        };

        toyotaCamry.Features.Add(sunroof);
        bmwX5.Features.Add(sunroof);
        bmwX5.Features.Add(heatedSeats);

        context.Countries.AddRange(japan, germany);
        context.Brands.AddRange(toyota, bmw);
        context.Features.AddRange(sunroof, heatedSeats);
        context.Cars.AddRange(toyotaCamry, bmwX5);

        await context.SaveChangesAsync(ct);
    }
}

