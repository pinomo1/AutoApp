using Microsoft.Extensions.Options;
using Renci.SshNet;

namespace AutoApp.Infrastructure.Services;

public class SftpBrandLogoStorage(IOptions<BrandLogoStorageOptions> options) : IBrandLogoStorage
{
    private readonly BrandLogoStorageOptions _options = options.Value;

    public Task<string> UploadAsync(Guid brandId, Stream content, string originalFileName, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(_options.Host) || string.IsNullOrWhiteSpace(_options.Username))
            throw new InvalidOperationException("SFTP configuration is invalid.");

        var extension = Path.GetExtension(originalFileName);
        if (string.IsNullOrWhiteSpace(extension))
        {
            extension = ".bin";
        }

        var fileName = $"{brandId:N}{extension.ToLowerInvariant()}";
        var remoteDirectory = NormalizeDirectory(_options.RemoteDirectory);
        var remotePath = $"{remoteDirectory}/{fileName}";

        using var client = new SftpClient(_options.Host, _options.Port, _options.Username, _options.Password);
        client.Connect();

        EnsureDirectoryExists(client, remoteDirectory);

        if (content.CanSeek)
        {
            content.Position = 0;
        }

        client.UploadFile(content, remotePath, true);
        client.Disconnect();

        var publicBaseUrl = _options.PublicBaseUrl?.Trim().TrimEnd('/');
        if (!string.IsNullOrWhiteSpace(publicBaseUrl))
        {
            return Task.FromResult($"{publicBaseUrl}/{fileName}");
        }

        return Task.FromResult(remotePath);
    }

    private static string NormalizeDirectory(string? remoteDirectory)
    {
        if (string.IsNullOrWhiteSpace(remoteDirectory))
            return "/upload";

        var normalized = remoteDirectory.Trim().Replace('\\', '/').TrimEnd('/');
        return normalized.StartsWith('/') ? normalized : "/" + normalized;
    }

    private static void EnsureDirectoryExists(SftpClient client, string remoteDirectory)
    {
        var parts = remoteDirectory.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var current = "/";

        foreach (var part in parts)
        {
            current = current.EndsWith('/') ? current + part : current + "/" + part;
            if (!client.Exists(current))
            {
                client.CreateDirectory(current);
            }
        }
    }
}
