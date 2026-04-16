namespace AutoApp.Application.DTOs.Responses.SharedResponses;

/// <summary>
/// Response payload for paginated queries
/// </summary>
/// <param name="Items">Returned items</param>
/// <param name="Page">Current page number</param>
/// <param name="PageSize">Number of items per page</param>
/// <param name="TotalCount">Total number of items matching the query</param>
/// <typeparam name="T">Item type</typeparam>
public record PaginatedResult<T>(
    IList<T> Items,
    int Page,
    int PageSize,
    int TotalCount)
{
    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    /// <summary>
    /// Indicates whether a next page exists
    /// </summary>
    public bool HasNextPage => Page < TotalPages;
    /// <summary>
    /// Indicates whether a previous page exists
    /// </summary>
    public bool HasPreviousPage => Page > 1;
}