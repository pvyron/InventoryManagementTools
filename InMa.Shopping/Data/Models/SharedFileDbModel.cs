using System.ComponentModel.DataAnnotations;

namespace InMa.Shopping.Data.Models;

public sealed class SharedFileDbModel
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();
    [Required]
    [Length(1, 200)]
    public required string FileName { get; set; }
    [Required]
    [Length(1, 30)]
    public required string FileExtension { get; set; }
    [Required]
    public required long FileSizeBytes{ get; set; }
    [Required]
    public required string Tags { get; set; }
    [Required]
    public required DateTimeOffset DateCaptured { get; set; }
    public DateTimeOffset UploadedOn { get; set; } = DateTimeOffset.UtcNow;
    
    [Required]
    public required ApplicationUser Uploader { get; set; }

    public ICollection<SharedFilesUsersLinkDbModel> SharedFileUsers { get; set; } = new List<SharedFilesUsersLinkDbModel>();

    public string BlobId => $"{Uploader.Id}/{Id}";
}