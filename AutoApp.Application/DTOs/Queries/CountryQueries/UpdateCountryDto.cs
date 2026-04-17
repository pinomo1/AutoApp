namespace AutoApp.Application.DTOs.Queries.CountryQueries;

/// <summary>
/// Request to update an existing country
/// </summary>
/// <param name="Id">Country GUID</param>
/// <param name="CountryName">New country name</param>
public record UpdateCountryDto(Guid Id, string CountryName);