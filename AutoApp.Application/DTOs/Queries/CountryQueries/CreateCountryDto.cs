namespace AutoApp.Application.DTOs.Queries.CountryQueries;

/// <summary>
/// Request to create a country
/// </summary>
/// <param name="CountryName">Country name</param>
/// <param name="CountryCode">Country code</param>
public record CreateCountryDto(string CountryName, string CountryCode);
