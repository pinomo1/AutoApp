using AutoApp.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoApp.Infrastructure.Persistence.Configurations;

public sealed class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(user => user.UserName).HasMaxNVarCharOpt(256);
        builder.Property(user => user.NormalizedUserName).HasMaxNVarCharOpt(256);
        builder.Property(user => user.Email).HasMaxNVarCharOpt(256);
        builder.Property(user => user.NormalizedEmail).HasMaxNVarCharOpt(256);
        builder.Property(user => user.PasswordHash).HasMaxNVarCharOpt(512);
        builder.Property(user => user.SecurityStamp).HasMaxNVarCharOpt(256);
        builder.Property(user => user.ConcurrencyStamp).HasMaxNVarCharOpt(256);
        builder.Property(user => user.PhoneNumber).HasMaxNVarCharOpt(32);
        builder.Property(user => user.RefreshTokenHash).HasMaxNVarCharOpt(128);
    }
}
