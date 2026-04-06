using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoApp.Infrastructure.Persistence.Configurations;

public static class ConfigurationExtension
{
    extension(PropertyBuilder builder)
    {
        public PropertyBuilder HasMaxNVarChar(int maxLength)
        {
            return builder.HasMaxLength(maxLength).HasColumnType("nvarchar(" + maxLength + ")").IsRequired();
        }
    }
}