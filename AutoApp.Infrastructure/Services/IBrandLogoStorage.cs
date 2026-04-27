namespace AutoApp.Infrastructure.Services;

public interface IBrandLogoStorage
{
    Task<string> UploadAsync(Guid brandId, Stream content, string originalFileName, CancellationToken ct);
}
