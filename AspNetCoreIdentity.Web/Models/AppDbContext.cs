using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreIdentity.Web.Models
{
    
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, string>
    {

        public DbSet<UserGroup> UserGroup { get; set; }
        public DbSet<Messages> Messages { get; set; }
        public DbSet<UserGroupMatch> UserGroupMatch { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
    }
}
