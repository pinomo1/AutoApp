using AutoApp.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoApp.Infrastructure.Persistence.Configurations;


public abstract class AbstractConfiguration<T> : IEntityTypeConfiguration<T> where T : AbstractModel
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedOnAdd().IsRequired();
    }
}