using System.Text.Json.Serialization;

namespace AutoApp.Application.DTOs.Queries;

/// <summary>
/// Type of sorting cars
/// </summary>
public enum CarSortType
{
    /// <summary>
    /// Ascending by year
    /// </summary>
    YearAscending,
    /// <summary>
    /// Descending by year
    /// </summary>
    YearDescending,
    /// <summary>
    /// Ascending by price
    /// </summary>
    PriceAscending,
    /// <summary>
    /// Descending by price
    /// </summary>
    PriceDescending,
    /// <summary>
    /// Ascending by mileage
    /// </summary>
    MileageAscending,
    /// <summary>
    /// Descending by mileage
    /// </summary>
    MileageDescending
}

/// <summary>
/// Used to sort search results
/// </summary>
/// <param name="SortType">Type of sorting</param>
[JsonConverter(typeof(JsonStringEnumConverter))]   
public record CarSorting(
    CarSortType? SortType
);