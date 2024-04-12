using InMa.Shopping.DomainModels;

namespace InMa.Shopping.Data.Repositories.Abstractions;

public interface IFilesRepository : IStartupProcess
{
    Task<string> UploadFile(Stream fileStream, UploadFileInfo uploadFileInfo, CancellationToken cancellationToken);
}

public sealed record UploadFileInfo
{
    public required string FileName { get; init; }
    public required string UploaderEmail { get; init; }
    public required IEnumerable<string> SharedFileUsers { get; init; }
}