using AutoApp.Domain.Abstractions;

namespace AutoApp.Domain.Entities;

public class Brand : AbstractModel
{
    public string BrandName { get; set; } = "";
    public string? LogoUrl { get; set; }
    public ICollection<Car> Cars { get; } = new List<Car>();
    public Country Country { get; set; } = new();
    public Guid  CountryId { get; set; }
}