using System.ComponentModel.DataAnnotations;

namespace AutoApp.Application.DTOs.Queries;

/// <summary>
/// Record for search filtering system
/// </summary>
/// <param name="Brand">Match the car on brand</param>
/// <param name="Color">Match the car on color</param>
/// <param name="Year">Match the car on year</param>
public record CarFilters(
    [StringLength(32)] string? Brand, 
    [StringLength(32)] string? Color, 
    [Range(1800, 2026)] short? Year
);