namespace AutoApp.Infrastructure.Services;

/// <summary>
/// Supported storage protocols for file uploads.
/// </summary>
public enum StorageProtocol
{
    /// <summary>
    /// SSH File Transfer Protocol (default).
    /// </summary>
    Sftp = 0,

    /// <summary>
    /// File Transfer Protocol.
    /// </summary>
    Ftp = 1,

    /// <summary>
    /// FTP over SSL/TLS.
    /// </summary>
    Ftps = 2
}
