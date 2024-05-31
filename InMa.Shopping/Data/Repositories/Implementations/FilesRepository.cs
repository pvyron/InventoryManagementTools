using System.Text.Json;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using InMa.Shopping.Data.Repositories.Abstractions;
using InMa.Shopping.Data.Repositories.Models;
using InMa.Shopping.DomainModels;
using Microsoft.EntityFrameworkCore;

namespace InMa.Shopping.Data.Repositories.Implementations;

public sealed class FilesRepository : IFilesRepository
{
    private readonly ILogger<FilesRepository> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly BlobContainerClient _containerClient;
    private readonly QueueClient _queueClient;

    public FilesRepository(ILogger<FilesRepository> logger, IConfiguration configuration, ApplicationDbContext dbContext, JsonSerializerOptions jsonSerializerOptions)
    {
        _logger = logger;
        _dbContext = dbContext;
        _jsonSerializerOptions = jsonSerializerOptions;
        _queueClient = new(configuration.GetConnectionString("StorageAccount"), "devfiles");
        _containerClient = new(configuration.GetConnectionString("StorageAccount"), "devfiles");
    }

    public async Task<string> UploadFile(Stream fileStream, UploadFileInfo uploadFileInfo, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Upload request from user: {user}", uploadFileInfo.UploaderEmail);

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == uploadFileInfo.UploaderEmail, cancellationToken: cancellationToken);

        if (user is null)
        {
            _logger.LogCritical("Unauthorized user: {userEmail} tried to upload", uploadFileInfo.UploaderEmail);
            return "";
        }

        var result = await UploadFileInternal(fileStream, user, uploadFileInfo, cancellationToken);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Successful upload from user: {user} of file: {fileName}", uploadFileInfo.UploaderEmail, result);
        return result;
    }

    public async Task<string[]> UploadFiles(Stream[] fileStreams, UploadFilesInfo uploadFilesInfo,
        CancellationToken cancellationToken)
    {
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
            uploadTasks.Add(UploadFileInternal(fileStreams[i], user, new UploadFileInfo
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

    private async Task<string> UploadFileInternal(Stream fileStream, ApplicationUser user, UploadFileInfo uploadFileInfo,
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

        if (uploadFileInfo.SharedFileUsers.Length > 0)
        {
            foreach (var sharedUserEmail in uploadFileInfo.SharedFileUsers)
            {
                var sharedUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == sharedUserEmail,
                    cancellationToken: cancellationToken);

                if (sharedUser is null || sharedUser.Id == user.Id)
                    continue;

                fileDb.Entity.SharedFileUsers.Add(new SharedFilesUsersLink()
                    { User = sharedUser, SharedFile = fileDb.Entity });
            }
        }

        var uploadResponse =
            await _containerClient.UploadBlobAsync(fileDb.Entity.BlobId, fileStream, cancellationToken);

        if (!uploadResponse.HasValue)
        {
            _logger.LogError("Upload failed with response: {response}",
                uploadResponse.GetRawResponse().Content.ToString());
            return "";
        }

        var queueMessage = new FileUploadQueueMessage
        {
            BlobId = fileDb.Entity.BlobId,
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

        return fileDb.Entity.BlobId;
    }

    public async Task Initialize()
    {
        await _queueClient.CreateIfNotExistsAsync();
        await _containerClient.CreateIfNotExistsAsync();
    }
}