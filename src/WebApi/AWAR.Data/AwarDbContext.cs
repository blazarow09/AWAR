namespace AWAR.Data
{
    using AWAR.Data.Models.User;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    public class AwarDbContext : IdentityDbContext
    {
        public AwarDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    }
}