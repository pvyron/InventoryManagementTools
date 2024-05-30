using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace InMa.Shopping.Data.Repositories.Models;

public sealed class SharedFileDbModel
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();
    [Required]
    [Length(1, 200)]
    public required string FileName { get; set; }
    [Required]
    public required long FileSizeBytes{ get; set; }
    [Required]
    public required string Tags { get; set; }
    [Required]
    public required DateTimeOffset DateCaptured { get; set; }
    public DateTimeOffset UploadedOn { get; set; } = DateTimeOffset.UtcNow;
    
    [Required]
    public required ApplicationUser Uploader { get; set; }

    public ICollection<SharedFilesUsersLink> SharedFileUsers { get; set; } = new List<SharedFilesUsersLink>();

    public string BlobId => $"{Uploader.Id}/{Id}";
}