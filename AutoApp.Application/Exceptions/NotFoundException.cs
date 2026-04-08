namespace AutoApp.Application.Exceptions;

/// <summary>
/// Exception for when the item was not found by ID
/// </summary>
/// <param name="name">Search item</param>
/// <param name="id">ID of the item</param>
public class NotFoundException(string name, Guid id) : Exception($"Couldn't find {name}  with id {id}");