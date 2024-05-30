namespace InMa.Shopping.Data.Repositories.Models;

public sealed class FileUploadQueueMessage
{
    public string? BlobId { get; set; }
    
    public FileUploadQueueMessageValues? Values { get; set; }
}

public sealed class FileUploadQueueMessageValues
{
    //metadata
    public string? OriginalName { get; set; }
    public string? ContentType { get; init; }
    public DateTimeOffset? LastModified { get; init; }
    
    //type specific metadata
    public string? CountryCode { get; init; }
    public string? Region { get; init; }
    public string? City { get; init; }
}