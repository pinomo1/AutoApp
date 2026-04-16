namespace AutoApp.Application.DTOs.Responses.SharedResponses;

/// <summary>
/// Response payload with items and total count
/// </summary>
/// <param name="Items">Returned items</param>
/// <param name="TotalCount">Total number of items matching the query</param>
/// <typeparam name="T">Item type</typeparam>
public record CountedResult<T>(IList<T> Items, int TotalCount);