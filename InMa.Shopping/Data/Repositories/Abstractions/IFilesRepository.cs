using InMa.Shopping.DomainModels;

namespace InMa.Shopping.Data.Repositories.Abstractions;

public interface IFilesRepository : IStartupProcess
{
    Task<string> UploadFile(string uploader, Stream fileStream, CancellationToken cancellationToken);
}