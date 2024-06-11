using InMa.Shopping.Data.Repositories.Models;
using InMa.Shopping.DomainModels;

namespace InMa.Shopping.Data.Repositories.Abstractions;

public interface IFilesRepository : IStartupProcess
{
    Task<string> UploadFile(Stream fileStream, UploadFileInfo uploadFileInfo, CancellationToken cancellationToken);
    Task<string[]> UploadFiles(Stream[] fileStreams, UploadFilesInfo uploadFilesInfo, CancellationToken cancellationToken);
    Task<Stream?> DownloadFile(string blobId, CancellationToken cancellationToken);
    Task<SearchFileResult[]> SearchFilesForUser(string uploaderEmail, CancellationToken cancellationToken);
}