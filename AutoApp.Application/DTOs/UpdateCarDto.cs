namespace AutoApp.Application.DTOs;

public record UpdateCarDto(Guid Id, string Brand, string Model, short Year, string Color, double Price, double Mileage);