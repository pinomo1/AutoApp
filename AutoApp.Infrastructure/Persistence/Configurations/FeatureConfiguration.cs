using AutoApp.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoApp.Infrastructure.Persistence.Configurations;

public class FeatureConfiguration : AbstractConfiguration<Feature>
{
    public override void Configure(EntityTypeBuilder<Feature> builder)
    {
        base.Configure(builder);
        builder.Property(f => f.FeatureName).HasMaxNVarChar(32).IsRequired();
        builder.HasMany(f => f.Cars)
            .WithMany(c => c.Features)
            .UsingEntity<CarFeature>();
    }
}