namespace AutoApp.API.Identity;

/// <summary>
///
/// </summary>
public sealed class IdentitySeedAdminOptions
{
    /// <summary>
    ///
    /// </summary>
    public const string SectionName = "Identity:SeedAdmin";

    /// <summary>
    ///
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    ///
    /// </summary>
    public string Role { get; set; } = "Admin";

    /// <summary>
    ///
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    ///
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    ///
    /// </summary>
    public string Password { get; set; } = string.Empty;
}
