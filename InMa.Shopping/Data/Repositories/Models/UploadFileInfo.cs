namespace InMa.Shopping.Data.Repositories.Models;

public sealed record UploadFileInfo
{
    public required string CountryCode { get; init; }
    public required string Region { get; init; }
    public required string City { get; init; }
    public required string UploaderEmail { get; init; }
    public required DateTime DateCaptured { get; init; }
    public required string[] Tags { get; init; }
    public required string[] SharedFileUsers { get; init; }
    
    public required FileProperties FileProperties { get; init; }
}