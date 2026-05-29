namespace AutoApp.Application.DTOs.Responses.SharedResponses;

/// <summary>
/// Response payload containing a resource ID
/// </summary>
/// <param name="Id">The ID of the created or updated resource</param>
public record IdResponse(Guid Id);
