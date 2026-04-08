using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace AutoApp.Application.DTOs.Queries;

/// <summary>
/// Represents a car to be updated
/// </summary>
/// <param name="Brand">Brand of the car</param>
/// <param name="Model">Model of the car</param>
/// <param name="Year">Year the car was assembled</param>
/// <param name="Color">Color of the car</param>
/// <param name="Price">Price requested by the seller</param>
/// <param name="Mileage">Mileage of the car</param>
public record UpdateCarDto(
    [Required]
    [StringLength(32)]
    string Brand,
    [Required]
    [StringLength(32)]
    string Model,
    [Required] 
    short Year,
    [Required]
    [StringLength(32)]
    string Color,
    [Required]
    [Range(typeof(decimal),"0", "100000000")]
    decimal Price,
    [Required]
    [Range(0, double.MaxValue)]
    double Mileage
);