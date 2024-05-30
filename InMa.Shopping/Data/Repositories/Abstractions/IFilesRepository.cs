﻿using InMa.Shopping.DomainModels;

namespace InMa.Shopping.Data.Repositories.Abstractions;

public interface IFilesRepository : IStartupProcess
{
    Task<string> UploadFile(Stream fileStream, UploadFileInfo uploadFileInfo, CancellationToken cancellationToken);
}

public sealed record UploadFileInfo
{
    //T-SQL
    public required string FileName { get; init; }
    public required string UploaderEmail { get; init; }
    public required long FileSizeBytes{ get; init; }
    public required DateTime DateCaptured { get; init; }
    public required string[] Tags { get; init; }
    public required string[] SharedFileUsers { get; init; }
    
    //metadata
    public required string OriginalName { get; set; }
    public required string ContentType { get; init; }
    public required DateTimeOffset LastModified { get; init; }
    
    //type specific metadata
    public required string CountryCode { get; init; }
    public required string Region { get; init; }
    public required string City { get; init; }
}