using AutoApp.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoApp.Infrastructure.Persistence.Configurations;

public class CountryConfiguration : AbstractConfiguration<Country>
{
    public override void Configure(EntityTypeBuilder<Country> builder)
    {
        base.Configure(builder);
        builder.Property(c => c.CountryName).HasMaxNVarChar(32).IsRequired();
        builder.Property(c => c.CountryCode).HasMaxNVarChar(2).IsRequired();
    }
}