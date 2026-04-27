namespace AutoApp.Application.DTOs.Queries.CountryQueries;

/// <summary>
/// Request to update an existing country
/// </summary>
/// <param name="CountryName">New country name</param>
/// <param name="CountryCode">New country code</param>
public record UpdateCountryDto(string CountryName, string CountryCode);
