using AutoApp.Domain.Enums;

namespace AutoApp.Application.DTOs.Queries.CarQueries;

/// <summary>
/// Request to create a car
/// </summary>
/// <param name="BrandId">Brand GUID</param>
/// <param name="Model">Car model name</param>
/// <param name="Year">Production year</param>
/// <param name="CarCondition">Vehicle condition</param>
/// <param name="CarType">Vehicle body type</param>
/// <param name="FuelType">Vehicle fuel type</param>
/// <param name="TransmissionType">Vehicle transmission type</param>
/// <param name="Color">Body color</param>
/// <param name="Horsepower">Engine power in horsepower (hp)</param>
/// <param name="EngineVolumeCc">Engine displacement in cubic centimeters (cc)</param>
/// <param name="Price">Listing price</param>
/// <param name="Mileage">Vehicle mileage</param>
public record CreateCarDto(
    Guid BrandId,
    string Model,
    short Year,
    CarCondition CarCondition,
    CarType CarType,
    FuelType FuelType,
    TransmissionType TransmissionType,
    Color Color,
    int Horsepower,
    int EngineVolumeCc,
    decimal Price,
    double Mileage
);