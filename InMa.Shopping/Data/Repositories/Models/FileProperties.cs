namespace InMa.Shopping.Data.Repositories.Models;

public sealed record FileProperties
{
    public required string FileName { get; init; }
    public required string FileExtension { get; init; }
    public required string OriginalName { get; set; }
    public required string ContentType { get; init; }
    public required long FileSizeBytes{ get; init; }
    public required DateTimeOffset LastModified { get; init; }
}