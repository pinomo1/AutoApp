using AutoApp.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoApp.Infrastructure.Persistence.Configurations;

public class CarConfiguration : AbstractConfiguration<Car>
{
    public override void Configure(EntityTypeBuilder<Car> builder)
    {
        base.Configure(builder);
        builder.Property(c => c.Brand).HasMaxNVarChar(32).IsRequired();
        builder.Property(c => c.Model).HasMaxNVarChar(32).IsRequired();
        builder.Property(c => c.Color).HasMaxNVarChar(32).IsRequired();
        builder.Property(c => c.Year).IsRequired();
        builder.Property(c => c.Price).IsRequired();
        builder.Property(c => c.Mileage).IsRequired();
    }
}