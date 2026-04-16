using System.ComponentModel.DataAnnotations;

namespace AutoApp.Application.DTOs.Queries.CountryQueries;

/// <summary>
/// Request to create a country
/// </summary>
/// <param name="CountryName">Country name</param>
public record CreateCountryDto([Required] [StringLength(32)] string CountryName);