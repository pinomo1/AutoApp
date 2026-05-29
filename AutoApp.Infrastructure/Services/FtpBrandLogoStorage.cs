using Microsoft.Extensions.Options;
using FluentFTP;
using System.Net;

namespace AutoApp.Infrastructure.Services;

/// <summary>
/// Brand logo storage implementation using FTP or FTPS protocols.
/// </summary>
public class FtpBrandLogoStorage(IOptions<BrandLogoStorageOptions> options) : IBrandLogoStorage
{
    private readonly BrandLogoStorageOptions _options = options.Value;

    public async Task<string> UploadAsync(Guid brandId, Stream content, string originalFileName, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(_options.Host) || string.IsNullOrWhiteSpace(_options.Username))
            throw new InvalidOperationException("FTP configuration is invalid.");

        var extension = Path.GetExtension(originalFileName);
        if (string.IsNullOrWhiteSpace(extension))
        {
            extension = ".bin";
        }

        var fileName = $"{brandId:N}{extension.ToLowerInvariant()}";
        var remoteDirectory = NormalizeDirectory(_options.RemoteDirectory);
        var remotePath = $"{remoteDirectory}/{fileName}";

        var credentials = new NetworkCredential(_options.Username, _options.Password);
        var config = new FtpConfig();

        // Configure SSL/TLS for FTPS
        if (_options.Protocol == StorageProtocol.Ftps)
        {
            config.EncryptionMode = FtpEncryptionMode.Explicit;
            config.ValidateAnyCertificate = true;
        }

        await using var client = new AsyncFtpClient(_options.Host, credentials, _options.Port, config);

        await client.Connect(ct);

        try
        {
            await EnsureDirectoryExists(client, remoteDirectory, ct);

            if (content.CanSeek)
            {
                content.Position = 0;
            }

            await client.UploadStream(content, remotePath, token: ct);
        }
        finally
        {
            await client.Disconnect(ct);
        }

        var publicBaseUrl = _options.PublicBaseUrl?.Trim().TrimEnd('/');
        return !string.IsNullOrWhiteSpace(publicBaseUrl) ? $"{publicBaseUrl}/{fileName}" : remotePath;
    }

    private static string NormalizeDirectory(string? remoteDirectory)
    {
        if (string.IsNullOrWhiteSpace(remoteDirectory))
            return "/upload";

        var normalized = remoteDirectory.Trim().Replace('\\', '/').TrimEnd('/');
        return normalized.StartsWith('/') ? normalized : "/" + normalized;
    }

    private static async Task EnsureDirectoryExists(AsyncFtpClient client, string remoteDirectory, CancellationToken ct)
    {
        var parts = remoteDirectory.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var current = "/";

        foreach (var part in parts)
        {
            current = current.EndsWith('/') ? current + part : current + "/" + part;
            var exists = await client.DirectoryExists(current, ct);
            if (!exists)
            {
                await client.CreateDirectory(current, ct);
            }
        }
    }
}
