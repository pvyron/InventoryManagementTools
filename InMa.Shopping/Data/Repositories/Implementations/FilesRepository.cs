using Azure.Storage.Blobs;
using InMa.Shopping.Data.Repositories.Abstractions;
using InMa.Shopping.DomainModels;

namespace InMa.Shopping.Data.Repositories.Implementations;

public sealed class FilesRepository : IFilesRepository
{
    private readonly ILogger<FilesRepository> _logger;
    private readonly BlobContainerClient _containerClient;

    public FilesRepository(ILogger<FilesRepository> logger, IConfiguration configuration)
    {
        _logger = logger;
        _containerClient = new(configuration.GetConnectionString("StorageAccount"), "devfiles");
    }

    public async Task<string> UploadFile(string uploader, Stream fileStream, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Upload request from user: {user}", uploader);
        
        var blobName = $"test/{EntityId.New()}".ToLower();
        await _containerClient.UploadBlobAsync(blobName, fileStream, cancellationToken);
        _logger.LogInformation("Successful upload from user: {user} of file: {fileName}", uploader, blobName);
        return blobName;
    }

    public async Task Initialize()
    {
        await _containerClient.CreateIfNotExistsAsync();
    }
}