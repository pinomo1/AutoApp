namespace AutoApp.Application.DTOs;

public record ResponseCarDto(Guid Id, string Brand, string Model, short Year, string Color, double Price, double Mileage);