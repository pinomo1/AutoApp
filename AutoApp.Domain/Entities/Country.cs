using AutoApp.Domain.Abstractions;

namespace AutoApp.Domain.Entities;

public class Country : AbstractModel
{
    public string CountryName { get; set; } = "";
    public string CountryCode { get; set; } = "";
    public ICollection<Brand> Brands { get; set; } = new List<Brand>();
}