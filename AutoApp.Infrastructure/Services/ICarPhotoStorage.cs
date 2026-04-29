namespace AutoApp.Infrastructure.Services;

public interface ICarPhotoStorage
{
    Task<string> UploadAsync(Guid carPhotoId, Stream content, string originalFileName, CancellationToken ct);
}
