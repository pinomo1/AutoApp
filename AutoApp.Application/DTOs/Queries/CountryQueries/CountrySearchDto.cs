using System.ComponentModel.DataAnnotations;

namespace AutoApp.Application.DTOs.Queries.CountryQueries;

/// <summary>
/// Search criteria for countries
/// </summary>
/// <param name="CountryName">Country name filter</param>
public record CountrySearchDto([StringLength(32)] string? CountryName);