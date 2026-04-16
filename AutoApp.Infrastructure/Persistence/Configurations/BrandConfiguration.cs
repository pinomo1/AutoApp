using AutoApp.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoApp.Infrastructure.Persistence.Configurations;

public class BrandConfiguration : AbstractConfiguration<Brand>
{
    public override void Configure(EntityTypeBuilder<Brand> builder)
    {
        base.Configure(builder);
        builder.Property(c => c.BrandName).HasMaxNVarChar(32).IsRequired();
        
        builder.HasOne(b => b.Country).WithMany(c => c.Brands).HasForeignKey(b => b.CountryId);
    }
}