namespace AutoApp.Domain.Abstractions;

public abstract class AbstractModel
{
    /// <summary>
    /// The unique identifier for this model.
    /// </summary>
    public Guid Id { get; set; }
}