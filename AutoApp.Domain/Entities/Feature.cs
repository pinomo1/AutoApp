using AutoApp.Domain.Abstractions;

namespace AutoApp.Domain.Entities;

public class Feature : AbstractModel
{
    public string FeatureName { get; set; } = "";
    public ICollection<Car> Cars { get; set; } = new List<Car>();
    public ICollection<CarFeature> CarFeatures { get; set; } = new List<CarFeature>();
}