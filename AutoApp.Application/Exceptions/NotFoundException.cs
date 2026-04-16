namespace AutoApp.Application.Exceptions;

/// <summary>
/// Thrown when an entity cannot be found by its identifier
/// </summary>
/// <param name="name">Entity name used in the error message</param>
/// <param name="id">Identifier value used for lookup</param>
public class NotFoundException(string name, Guid id) : Exception($"Couldn't find {name}  with id {id}");