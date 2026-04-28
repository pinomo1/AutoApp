using System.ComponentModel.DataAnnotations;

namespace AutoApp.Infrastructure.Services;

public class BrandLogoStorageOptions
{
    public const string SectionName = "BrandLogoStorage";

    [Required]
    public string Host { get; set; } = "localhost";

    [Range(1, 65535)]
    public int Port { get; set; } = 22;

    [Required]
    public string Username { get; set; } = "";

    [Required]
    public string Password { get; set; } = "";

    public string RemoteDirectory { get; set; } = "/upload";

    // Optional: when set, responses store a public URL instead of raw remote path.
    public string? PublicBaseUrl { get; set; }
}
