using System.ComponentModel.DataAnnotations;

namespace AutoApp.Application.DTOs.Queries.CountryQueries;

/// <summary>
/// Request to update an existing country
/// </summary>
/// <param name="Id">Country GUID</param>
/// <param name="CountryName">New country name</param>
public record UpdateCountryDto([Required] Guid Id, [Required] [StringLength(32)] string CountryName);