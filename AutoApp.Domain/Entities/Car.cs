using System.ComponentModel.DataAnnotations.Schema;
using AutoApp.Domain.Abstractions;
using AutoApp.Domain.Enums;

namespace AutoApp.Domain.Entities;

public class Car : AbstractModel
{
    public Brand Brand { get; set; } = new();

    public Guid BrandId { get; set; }

    public string Model { get; set; } = "";

    public short Year { get; set; }

    public CarCondition CarCondition { get; set; } = CarCondition.Undefined;

    public CarType CarType { get; set; } = CarType.Undefined;

    public FuelType FuelType { get; set; } = FuelType.Undefined;

    public TransmissionType TransmissionType { get; set; } = TransmissionType.Undefined;

    public Color Color { get; set; } = Color.Undefined;

    public int Horsepower { get; set; }

    public int EngineVolumeCc { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }
    public double Mileage { get; set; }

    public ICollection<CarFeature> CarFeatures { get; set; } = new List<CarFeature>();
    public ICollection<Feature> Features { get; set; } = new List<Feature>();
    public ICollection<CarPhoto> CarPhotos { get; set; } = new List<CarPhoto>();
}
