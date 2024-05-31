using System.Text.Json;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
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

        var result =
            await UploadFileInternal(fileStream, fileDb.Entity.BlobId, user, uploadFileInfo, cancellationToken);

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
            var fileDb = _dbContext.SharedFiles.Add(new SharedFileDbModel()
            {
                FileName = uploadFilesInfo.FilesProperties[i].FileName,
                FileExtension = uploadFilesInfo.FilesProperties[i].FileExtension,
                Uploader = user,
                FileSizeBytes = uploadFilesInfo.FilesProperties[i].FileSizeBytes,
                DateCaptured = uploadFilesInfo.DateCaptured.ToUniversalTime(),
                Tags = string.Join(" ", uploadFilesInfo.Tags)
            });

            foreach (var sharedUserEmail in uploadFilesInfo.SharedFileUsers)
            {
                var sharedUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == sharedUserEmail,
                    cancellationToken: cancellationToken);

                if (sharedUser is null || sharedUser.Id == user.Id)
                    continue;

                fileDb.Entity.SharedFileUsers.Add(new SharedFilesUsersLinkDbModel()
                    { User = sharedUser, SharedFile = fileDb.Entity });
            }

            uploadTasks.Add(UploadFileInternal(fileStreams[i], fileDb.Entity.BlobId, user, new UploadFileInfo
            {
                City = uploadFilesInfo.City,
                CountryCode = uploadFilesInfo.CountryCode,
                DateCaptured = uploadFilesInfo.DateCaptured,
                FileProperties = uploadFilesInfo.FilesProperties[i],
                Region = uploadFilesInfo.Region,
                SharedFileUsers = uploadFilesInfo.SharedFileUsers,
                Tags = uploadFilesInfo.Tags,
                UploaderEmail = uploadFilesInfo.UploaderEmail
            }, cancellationToken));
        }

        // TODO replace with Task.WhenEach() on .net 9
        var results = await Task.WhenAll(uploadTasks);

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successful upload from user: {user} of files: {fileName}",
            uploadFilesInfo.UploaderEmail, string.Join(" ", results));

        return results;
    }

    private async Task<string> UploadFileInternal(Stream fileStream, string blobId, ApplicationUser user,
        UploadFileInfo uploadFileInfo,
        CancellationToken cancellationToken)
    {
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

    public async Task Initialize()
    {
        await _queueClient.CreateIfNotExistsAsync();
        await _containerClient.CreateIfNotExistsAsync();
    }
}