using Microsoft.AspNetCore.Identity;

namespace AutoApp.Infrastructure.Identity;

public sealed class ApplicationUser : IdentityUser<Guid>
{
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? RefreshTokenHash { get; set; }

    public DateTimeOffset? RefreshTokenExpiresAt { get; set; }
}
