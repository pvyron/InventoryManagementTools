using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace InMa.Shopping.Data.Repositories.Models;

public sealed class SharedFileDbModel
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();
    [Required]
    [Length(1, 200)]
    public required string FileName { get; set; }
    public DateTimeOffset UploadedOn { get; set; } = DateTimeOffset.UtcNow;
    public required ApplicationUser Uploader { get; set; }
    public ICollection<ApplicationUser> SharedFileUsers { get; set; } = new List<ApplicationUser>();

    public string BlobId => $"{Uploader.Id}/{Id}";
}