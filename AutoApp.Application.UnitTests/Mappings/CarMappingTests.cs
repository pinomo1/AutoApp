using AutoApp.Application.DTOs.Queries.CarQueries;
using AutoApp.Application.Mappings;
using AutoApp.Domain.Entities;
using AutoApp.Domain.Enums;

namespace AutoApp.Application.UnitTests.Mappings;

public class CarMappingTests
{
    [Test]
    public void ToEntity_WhenCreateDtoProvided_ShouldTrimModelAndMapValues()
    {
        var dto = new CreateCarDto(
            Guid.NewGuid(),
            "  Civic  ",
            2023,
            CarCondition.New,
            CarType.Sedan,
            FuelType.Gasoline,
            TransmissionType.Automatic,
            Color.Black,
            158,
            2000,
            25000m,
            12.5);

        var entity = dto.ToEntity();

        Assert.Multiple(() =>
        {
            Assert.That(entity.Model, Is.EqualTo("Civic"));
            Assert.That(entity.BrandId, Is.EqualTo(dto.BrandId));
            Assert.That(entity.Year, Is.EqualTo(dto.Year));
            Assert.That(entity.CarType, Is.EqualTo(dto.CarType));
            Assert.That(entity.Price, Is.EqualTo(dto.Price));
        });
    }

    [Test]
    public void ToListItemDto_WhenCarProvided_ShouldMapBrandAndColor()
    {
        var car = new Car
        {
            Id = Guid.NewGuid(),
            Model = "Civic",
            Year = 2023,
            Brand = new Brand { BrandName = "Honda" },
            Color = Color.Blue,
            Horsepower = 158,
            EngineVolumeCc = 2000,
            Price = 25000m,
            Mileage = 12.5
        };

        var dto = car.ToListItemDto();

        Assert.Multiple(() =>
        {
            Assert.That(dto.BrandName, Is.EqualTo("Honda"));
            Assert.That(dto.Color, Is.EqualTo("Blue"));
            Assert.That(dto.Model, Is.EqualTo("Civic"));
        });
    }
}