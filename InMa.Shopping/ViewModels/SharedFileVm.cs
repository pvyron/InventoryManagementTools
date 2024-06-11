using InMa.Shopping.DomainExtensions;

namespace InMa.Shopping.ViewModels;

public sealed class SharedFileVm
{
    public string Tags { get; set; } = string.Empty;
    public string ShareWith { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public DateTime? DateCaptured { get; set; } = DateTime.Now;

    public SharedFileInputProperties[] FileProperties { get; set; } = [];
}

public sealed class SharedFileInputProperties
{
    public required string Name { get; set; }
    public required string OriginalName { get; set; }
    public required string ContentType { get; init; }
    public required DateTimeOffset LastModified { get; init; }

    private long _fileSizeBytes;
    public required long FileSizeBytes
    {
        get => _fileSizeBytes;
        init
        {
            _fileSizeBytes = value;
            _fileSize = _fileSizeBytes.BytesAsText();
        }
    }

    private string? _fileSize;
    public string FileSize
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(_fileSize))
            {
                return _fileSize;
            }

            return _fileSize = _fileSizeBytes.BytesAsText();
        }
    }
}