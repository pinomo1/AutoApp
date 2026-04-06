namespace AutoApp.Application.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string name, Guid id) : base($"Couldn't find {name}  with id {id}"){}
}