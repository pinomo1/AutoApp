using AutoApp.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoApp.Infrastructure.Persistence.Configurations;

public class CarPhotoConfiguration : AbstractConfiguration<CarPhoto>
{
    public override void Configure(EntityTypeBuilder<CarPhoto> builder)
    {
        base.Configure(builder);
        builder.Property(p => p.PhotoUrl).HasMaxNVarChar(512).IsRequired();
        builder.Property(p => p.DisplayOrder).IsRequired();
        builder.Property(p => p.IsMainPhoto).IsRequired();

        builder.HasOne(p => p.Car).WithMany(c => c.CarPhotos).HasForeignKey(p => p.CarId);
    }
}
