using AutoApp.Application.DTOs.Queries.CarQueries;
using AutoApp.Application.DTOs.Responses.CarResponses;
using AutoApp.Domain.Entities;

namespace AutoApp.Application.Mappings;

/// <summary>
/// Mapping helpers for car entities and DTOs
/// </summary>
public static class CarMapping
{
    extension(Car car)
    {
        /// <summary>
        /// Maps a car entity to a detailed response DTO
        /// </summary>
        /// <returns>Detailed car response DTO</returns>
        public CarDetailsResponseDto ToDto() => new(car.Id, car.BrandId, car.Brand.BrandName, car.Model, car.Year,
            car.CarCondition, car.CarType, car.FuelType, car.TransmissionType,
            car.Color, car.Horsepower, car.EngineVolumeCc, car.Price, car.Mileage, car.Features.Select(f => f.ToDto()).ToList());

        /// <summary>
        /// Maps a car entity to a list item response DTO
        /// </summary>
        /// <returns>Car list item response DTO</returns>
        public CarListItemResponseDto ToListItemDto(string mainPhotoUrl) => new(car.Id, car.Brand.BrandName, car.Model, car.Year,
            car.Color.ToString(), car.Horsepower, car.EngineVolumeCc, car.Price, car.Mileage, mainPhotoUrl);
    }

    extension(CreateCarDto dto)
    {
        /// <summary>
        /// Maps a create-car request DTO to a new car entity
        /// </summary>
        /// <returns>Car entity ready to be persisted</returns>
        public Car ToEntity() => new()
        {
            Id = Guid.Empty,
            BrandId = dto.BrandId,
            Model = dto.Model.Trim(),
            Year = dto.Year,
            CarCondition = dto.CarCondition,
            CarType = dto.CarType,
            FuelType = dto.FuelType,
            TransmissionType = dto.TransmissionType,
            Color = dto.Color,
            Horsepower = dto.Horsepower,
            EngineVolumeCc = dto.EngineVolumeCc,
            Price = dto.Price,
            Mileage = dto.Mileage
        };
    }

    extension(UpdateCarDto dto)
    {
        /// <summary>
        /// Maps an update-car request DTO to a car entity
        /// </summary>
        /// <param name="id">Car identifier from route</param>
        /// <returns>Car entity with updated values</returns>
        public Car ToEntity(Guid id) => new()
        {
            Id = id,
            BrandId = dto.BrandId,
            Model = dto.Model.Trim(),
            Year = dto.Year,
            CarCondition = dto.CarCondition,
            CarType = dto.CarType,
            FuelType = dto.FuelType,
            TransmissionType = dto.TransmissionType,
            Color = dto.Color,
            Horsepower = dto.Horsepower,
            EngineVolumeCc = dto.EngineVolumeCc,
            Price = dto.Price,
            Mileage = dto.Mileage
        };
    }
}
