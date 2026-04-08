using AutoApp.Application.DTOs.Responses;
using AutoApp.Domain.Entities;

namespace AutoApp.Application.Mappings;

public static class CarMapping
{
    extension(Car car)
    {
        public ResponseCarDto ToDto() => new(car.Id, car.Brand, car.Model, car.Year, car.Color, car.Price, car.Mileage);
    }

    extension(IEnumerable<Car> cars)
    {
        public List<ResponseCarDto> ToDtoList()
            => cars.Select(c => c.ToDto()).ToList();
    }

    extension(ResponseCarDto dto)
    {
        public Car ToEntity() => new()
        {
            Id = dto.Id,
            Brand = dto.Brand,
            Model = dto.Model,
            Year = dto.Year,
            Color = dto.Color,
            Price = dto.Price,
            Mileage = dto.Mileage
        };
    }
}