using System.ComponentModel.DataAnnotations.Schema;
using InMa.Shopping.Data.Repositories.Models;
using Microsoft.AspNetCore.Identity;

namespace InMa.Shopping.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public ICollection<SharedFileDbModel> UploadedFiles { get; set; } = new List<SharedFileDbModel>();

    public ICollection<SharedFilesUsersLink> SharedFiles { get; set; } = new List<SharedFilesUsersLink>();
}