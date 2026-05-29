using System.ComponentModel.DataAnnotations;

namespace AutoApp.API.Identity;

/// <summary>
///
/// </summary>
public sealed class JwtOptions
{
    /// <summary>
    ///
    /// </summary>
    public const string SectionName = "Jwt";

    /// <summary>
    ///
    /// </summary>
    [Required]
    [MinLength(32)]
    public string SigningKey { get; set; } = string.Empty;

    /// <summary>
    ///
    /// </summary>
    [Required]
    public string Issuer { get; set; } = "AutoApp";

    /// <summary>
    ///
    /// </summary>
    [Required]
    public string Audience { get; set; } = "AutoApp";

    /// <summary>
    ///
    /// </summary>
    [Range(1, 1440)]
    public int ExpiresMinutes { get; set; } = 120;
}
