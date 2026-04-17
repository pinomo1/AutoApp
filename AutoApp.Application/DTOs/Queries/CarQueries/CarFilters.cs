using AutoApp.Domain.Enums;

namespace AutoApp.Application.DTOs.Queries.CarQueries;

/// <summary>
/// Search filters for cars
/// </summary>
/// <param name="SearchString">Full-text filter across searchable car fields</param>
/// <param name="BrandName">Brand name filter</param>
/// <param name="BrandId">Brand GUID filter</param>
/// <param name="CarCondition">Condition filter</param>
/// <param name="CarType">Body type filter</param>
/// <param name="FuelType">Fuel type filter</param>
/// <param name="TransmissionType">Transmission filter</param>
/// <param name="Color">Color filter</param>
/// <param name="Year">Production year filter</param>
public record CarFilters(
    string? SearchString,
    string? BrandName,
    Guid? BrandId,
    CarCondition? CarCondition,
    CarType? CarType,
    FuelType? FuelType,
    TransmissionType? TransmissionType,
    Color? Color,
    short? Year
)
{
    /// <summary>
    /// Creates an empty filter set with all values unset
    /// </summary>
    public CarFilters() : this(null, null, null, null, null, null, null, null, null) {}
}