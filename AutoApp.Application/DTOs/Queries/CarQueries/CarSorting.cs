using System.Text.Json.Serialization;

namespace AutoApp.Application.DTOs.Queries.CarQueries;

/// <summary>
/// Available sorting options for car search
/// </summary>
public enum CarSortType
{
    /// <summary>
    /// Undefined
    /// </summary>
    Undefined,
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
/// Sorting settings for car search
/// </summary>
/// <param name="SortType">Selected sorting option</param>
[JsonConverter(typeof(JsonStringEnumConverter))]
public record CarSorting(
    CarSortType? SortType
)
{
    /// <summary>
    /// Creates an empty sorting configuration
    /// </summary>
    public CarSorting() : this(SortType: null) {}
}