using AutoApp.Domain.Abstractions;

namespace AutoApp.Domain.Entities;

/// <summary>
/// This class represents a car
/// </summary>
public class Car : AbstractModel
{
    public Car()
    {
        Brand = "";
        Color = "";
        Price = 0;
        Mileage = 0;
        Model = "";
        Year = 0;
    }
    
    /// <summary>
    /// The brand of teh car
    /// </summary>
    public string Brand { get; set; }
    /// <summary>
    /// The model of the car
    /// </summary>
    public string Model { get; set; }
    /// <summary>
    /// The year the car was assembled
    /// </summary>
    public short Year { get; set; }
    /// <summary>
    /// Description of the car's color
    /// </summary>
    public string Color { get; set; }
    /// <summary>
    /// Price the seller demands
    /// </summary>
    public double Price { get; set; }
    /// <summary>
    /// Mileage of the car
    /// </summary>
    public double Mileage { get; set; }
}