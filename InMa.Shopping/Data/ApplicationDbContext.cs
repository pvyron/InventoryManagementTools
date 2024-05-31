using InMa.Shopping.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InMa.Shopping.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<SharedFileDbModel> SharedFiles { get; set; }
    public DbSet<SharedFilesUsersLinkDbModel> SharedFilesUsersLinks { get; set; }
}