using AutoApp.Domain.Abstractions;

namespace AutoApp.Domain.Entities;

public class CarPhoto : AbstractModel
{
    public Guid CarId { get; set; }
    public Car Car { get; set; } = new();
    public string PhotoUrl { get; set; } = "";
    public int DisplayOrder { get; set; }
    public bool IsMainPhoto { get; set; }
}
