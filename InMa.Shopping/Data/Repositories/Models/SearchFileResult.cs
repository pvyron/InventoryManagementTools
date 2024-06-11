namespace InMa.Shopping.Data.Repositories.Models;

public readonly record struct SearchFileResult
{
    public required Guid Id { get; init; }
    public required string FileName { get; init; }
    public required string FileExtension { get; init; }
    public required long FileSizeBytes{ get; init; }
    public required string Tags { get; init; }
    public required DateTimeOffset DateCaptured { get; init; }
    public required DateTimeOffset UploadedOn { get; init; }
    public required string UploaderId { get; init; }
    public string FullName => $"{FileName}{FileExtension}";
    public string BlobId => $"{UploaderId}/{Id}";
}