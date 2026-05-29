using System.ComponentModel.DataAnnotations;

namespace AutoApp.Infrastructure.Services;

public class CarPhotoStorageOptions
{
    public const string SectionName = "SharedFtpStorage";

    /// <summary>
    /// Storage protocol to use (Sftp, Ftp, or Ftps).
    /// </summary>
    public StorageProtocol Protocol { get; set; } = StorageProtocol.Sftp;

    [Required]
    public string Host { get; set; } = "localhost";

    [Range(1, 65535)]
    public int Port { get; set; } = 22;

    [Required]
    public string Username { get; set; } = "";

    [Required]
    public string Password { get; set; } = "";

    public string RemoteDirectory { get; set; } = "/upload";

    public string? PublicBaseUrl { get; set; }
}
