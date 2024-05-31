using System.ComponentModel.DataAnnotations;

namespace InMa.Shopping.Data.Models;

public class SharedFilesUsersLinkDbModel
{
    [Key] 
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required]
    public required SharedFileDbModel SharedFile { get; set; }
    [Required]
    public required ApplicationUser User { get; set; }
}