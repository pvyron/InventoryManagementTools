using System.Text.Json;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using InMa.Shopping.Data.Models;
using InMa.Shopping.Data.Repositories.Abstractions;
using InMa.Shopping.Data.Repositories.Models;
using Microsoft.EntityFrameworkCore;

namespace InMa.Shopping.Data.Repositories.Implementations;

public sealed class FilesRepository : IFilesRepository
{
    private readonly ILogger<FilesRepository> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly BlobContainerClient _containerClient;
    private readonly QueueClient _queueClient;

    public FilesRepository(ILogger<FilesRepository> logger, IConfiguration configuration,
        ApplicationDbContext dbContext, JsonSerializerOptions jsonSerializerOptions)
    {
        _logger = logger;
        _dbContext = dbContext;
        _jsonSerializerOptions = jsonSerializerOptions;
        _queueClient = new(configuration.GetConnectionString("StorageAccount"), "devfiles");
        _containerClient = new(configuration.GetConnectionString("StorageAccount"), "devfiles");
    }

    public async Task<string> UploadFile(Stream fileStream, UploadFileInfo uploadFileInfo,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Upload request from user: {user}", uploadFileInfo.UploaderEmail);

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == uploadFileInfo.UploaderEmail,
            cancellationToken: cancellationToken);

        if (user is null)
        {
            _logger.LogCritical("Unauthorized user: {userEmail} tried to upload", uploadFileInfo.UploaderEmail);
            return "";
        }

        var blobId = await PersistUploadFileInternal(user, uploadFileInfo, cancellationToken);

        var result = await UploadFileInternal(fileStream, blobId, uploadFileInfo, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successful upload from user: {user} of file: {fileName}", uploadFileInfo.UploaderEmail,
            result);
        return result;
    }

    public async Task<string[]> UploadFiles(Stream[] fileStreams, UploadFilesInfo uploadFilesInfo,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Upload request from user: {user}", uploadFilesInfo.UploaderEmail);

        if (fileStreams.Length != uploadFilesInfo.FilesProperties.Length)
        {
            _logger.LogCritical("Tried to upload multiple files with different number of streams and properties");
            return [""];
        }

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == uploadFilesInfo.UploaderEmail,
            cancellationToken: cancellationToken);

        if (user is null)
        {
            _logger.LogCritical("Unauthorized user: {userEmail} tried to upload", uploadFilesInfo.UploaderEmail);
            return [""];
        }

        List<Task<string>> uploadTasks = [];
        for (int i = 0; i < uploadFilesInfo.FilesProperties.Length; i++)
        {
            var fileInfo = new UploadFileInfo
            {
                City = uploadFilesInfo.City,
                CountryCode = uploadFilesInfo.CountryCode,
                DateCaptured = uploadFilesInfo.DateCaptured,
                FileProperties = uploadFilesInfo.FilesProperties[i],
                Region = uploadFilesInfo.Region,
                SharedFileUsers = uploadFilesInfo.SharedFileUsers,
                Tags = uploadFilesInfo.Tags,
                UploaderEmail = uploadFilesInfo.UploaderEmail
            };

            var blobId = await PersistUploadFileInternal(user, fileInfo, cancellationToken);

            uploadTasks.Add(UploadFileInternal(fileStreams[i], blobId, fileInfo, cancellationToken));
        }

        // TODO replace with Task.WhenEach() on .net 9
        var results = await Task.WhenAll(uploadTasks);

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successful upload from user: {user} of files: {fileName}",
            uploadFilesInfo.UploaderEmail, string.Join(" ", results));

        return results;
    }

    public async Task<Stream?> DownloadFile(string blobId, CancellationToken cancellationToken)
    {
        var blobClient = _containerClient.GetBlobClient(blobId);
        var downloadStreamingResponse = await blobClient.DownloadStreamingAsync(cancellationToken: cancellationToken);
        
        return downloadStreamingResponse?.Value.Content;
    }

    public async Task<SearchFileResult[]> SearchFilesForUser(string uploaderEmail, CancellationToken cancellationToken)
    {
        //_logger.LogInformation("Upload request from user: {user}", uploadFileInfo.UploaderEmail);

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == uploaderEmail,
            cancellationToken: cancellationToken);

        if (user is null)
        {
            //_logger.LogCritical("Unauthorized user: {userEmail} tried to upload", uploadFileInfo.UploaderEmail);
            return [];
        }

        var results = await _dbContext.SharedFiles
            .Where(f => f.Uploader.Id == user.Id)
            .Select(f => new SearchFileResult
            {
                DateCaptured = f.DateCaptured,
                FileExtension = f.FileExtension,
                FileName = f.FileName,
                FileSizeBytes = f.FileSizeBytes,
                Id = f.Id,
                Tags = f.Tags,
                UploadedOn = f.UploadedOn,
                UploaderId = f.Uploader.Id
            })
            .Take(15)
            .ToArrayAsync(cancellationToken);

        return results;
    }

    private async Task<string> UploadFileInternal(Stream fileStream, string blobId, UploadFileInfo uploadFileInfo,
        CancellationToken cancellationToken)
    {
        // TODO replace the upload with zipped
        // using var memoryStream = new MemoryStream();
        // using var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create);
        // var zipEntry = zipArchive.CreateEntry(uploadFileInfo.FileProperties.OriginalName);
        //
        // await using var zipStream = zipEntry.Open();
        // await fileStream.CopyToAsync(zipStream, cancellationToken);
        // memoryStream.Seek(0, SeekOrigin.Begin);
        //
        // var uploadResponse = await _containerClient.UploadBlobAsync(blobId, memoryStream, cancellationToken);
        
        var uploadResponse = await _containerClient.UploadBlobAsync(blobId, fileStream, cancellationToken);

        if (!uploadResponse.HasValue)
        {
            _logger.LogError("Upload failed with response: {response}",
                uploadResponse.GetRawResponse().Content.ToString());
            return "";
        }

        var queueMessage = new FileUploadQueueMessage
        {
            BlobId = blobId,
            Values = new()
            {
                City = uploadFileInfo.City,
                ContentType = uploadFileInfo.FileProperties.ContentType,
                CountryCode = uploadFileInfo.CountryCode,
                LastModified = uploadFileInfo.FileProperties.LastModified,
                OriginalName = uploadFileInfo.FileProperties.OriginalName,
                Region = uploadFileInfo.Region
            }
        };

        await _queueClient.SendMessageAsync(JsonSerializer.Serialize(queueMessage, _jsonSerializerOptions),
            cancellationToken);

        return blobId;
    }

    private async Task<string> PersistUploadFileInternal(ApplicationUser user, UploadFileInfo uploadFileInfo,
        CancellationToken cancellationToken)
    {
        var fileDb = _dbContext.SharedFiles.Add(new SharedFileDbModel()
        {
            FileName = uploadFileInfo.FileProperties.FileName,
            FileExtension = uploadFileInfo.FileProperties.FileExtension,
            Uploader = user,
            FileSizeBytes = uploadFileInfo.FileProperties.FileSizeBytes,
            DateCaptured = uploadFileInfo.DateCaptured.ToUniversalTime(),
            Tags = string.Join(" ", uploadFileInfo.Tags)
        });
        
        foreach (var sharedUserEmail in uploadFileInfo.SharedFileUsers)
        {
            var sharedUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == sharedUserEmail,
                cancellationToken: cancellationToken);

            if (sharedUser is null || sharedUser.Id == user.Id)
                continue;

            fileDb.Entity.SharedFileUsers.Add(new SharedFilesUsersLinkDbModel()
                { User = sharedUser, SharedFile = fileDb.Entity });
        }
        
        return fileDb.Entity.BlobId;
    }

    public async Task Initialize()
    {
        await _queueClient.CreateIfNotExistsAsync();
        await _containerClient.CreateIfNotExistsAsync();
    }
}