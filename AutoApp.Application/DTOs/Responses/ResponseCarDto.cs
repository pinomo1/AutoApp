namespace AutoApp.Application.DTOs.Responses;

/// <summary>
/// Returns a car
/// </summary>
/// <param name="Id">GUID of the car</param>
/// <param name="Brand">Brand of the car</param>
/// <param name="Model">Model of the car</param>
/// <param name="Year">Year the car was assembled</param>
/// <param name="Color">Color of the car</param>
/// <param name="Price">Price requested by the seller</param>
/// <param name="Mileage">Mileage of the car</param>
public record ResponseCarDto(Guid Id, string Brand, string Model, short Year, string Color, decimal Price, double Mileage);