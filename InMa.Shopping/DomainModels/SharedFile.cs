using System.Security.Cryptography;
using System.Text;

namespace InMa.Shopping.DomainModels;

public sealed class SharedFile : Entity
{
    public required string FileName { get; set; }
    
    public required string OriginalName { get; set; }
    public required string OriginalExtension { get; set; }
    public string OriginalFullName => $"{OriginalName}.{OriginalExtension}";
    
    private string? _uploader;
    private string? _uploaderHash;
    public required string Uploader
    {
        get => _uploader!;
        set
        {
            _uploaderHash = null;
            _uploader = value;
        }
    }
    
    public required List<string> FileGang { get; set; }
    
    public required DateTimeOffset UploadedOn { get; set; }
    
    public string BlobName
    {
        get
        {
            if (_uploaderHash is null)
            {
                var encoded = SHA3_512.HashData(Encoding.UTF8.GetBytes(Uploader));
                _uploaderHash = Convert.ToBase64String(encoded);
            }
            
            return $"{_uploaderHash}/{Id}";
        }
    }
}