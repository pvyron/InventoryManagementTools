﻿using Azure.Storage.Blobs;
using InMa.Shopping.Data.Repositories.Abstractions;
using InMa.Shopping.Data.Repositories.Models;
using InMa.Shopping.DomainModels;
using Microsoft.EntityFrameworkCore;

namespace InMa.Shopping.Data.Repositories.Implementations;

public sealed class FilesRepository : IFilesRepository
{
    private readonly ILogger<FilesRepository> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly BlobContainerClient _containerClient;

    public FilesRepository(ILogger<FilesRepository> logger, IConfiguration configuration, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
        _containerClient = new(configuration.GetConnectionString("StorageAccount"), "devfiles");
    }

    public async Task<string> UploadFile(Stream fileStream, UploadFileInfo uploadFileInfo, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Upload request from user: {user}", uploadFileInfo.UploaderEmail);

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == uploadFileInfo.UploaderEmail, cancellationToken: cancellationToken);

        var fileDb = _dbContext.SharedFiles.Add(new SharedFileDbModel()
        {
            FileName = uploadFileInfo.FileName,
            Uploader = user!
        });
        
        await _containerClient.UploadBlobAsync(fileDb.Entity.BlobId, fileStream, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Successful upload from user: {user} of file: {fileName}", uploadFileInfo.UploaderEmail, fileDb.Entity.BlobId);
        return fileDb.Entity.BlobId;
    }

    public async Task Initialize()
    {
        await _containerClient.CreateIfNotExistsAsync();
    }
}