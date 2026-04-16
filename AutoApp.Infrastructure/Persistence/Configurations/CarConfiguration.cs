using AutoApp.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoApp.Infrastructure.Persistence.Configurations;

public class CarConfiguration : AbstractConfiguration<Car>
{
    public override void Configure(EntityTypeBuilder<Car> builder)
    {
        base.Configure(builder);
        builder.Property(c => c.Model).HasMaxNVarChar(32).IsRequired();
        builder.Property(c => c.CarCondition).IsRequired();
        builder.Property(c => c.CarType).IsRequired();
        builder.Property(c => c.FuelType).IsRequired();
        builder.Property(c => c.TransmissionType).IsRequired();
        builder.Property(c => c.Color).IsRequired();
        builder.Property(c => c.Year).IsRequired();
        builder.Property(c => c.Horsepower).IsRequired();
        builder.Property(c => c.EngineVolumeCc).IsRequired();
        builder.Property(c => c.Price).IsRequired();
        builder.Property(c => c.Mileage).IsRequired();
        
        builder.HasOne(c => c.Brand).WithMany(b => b.Cars).HasForeignKey(c => c.BrandId);
    }
}