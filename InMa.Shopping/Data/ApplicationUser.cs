using InMa.Shopping.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace InMa.Shopping.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public ICollection<SharedFileDbModel> UploadedFiles { get; set; } = new List<SharedFileDbModel>();

    public ICollection<SharedFilesUsersLinkDbModel> SharedFiles { get; set; } = new List<SharedFilesUsersLinkDbModel>();
}