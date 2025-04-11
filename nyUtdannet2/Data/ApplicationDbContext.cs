using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using nyUtdannet2.Models;

namespace nyUtdannet2.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<JobListing> JobListings { get; set; }
        public DbSet<JobApp> JobApps { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        
        // Legg til disse nye DbSet-ene
        public DbSet<EmployerUser> EmployerUsers { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            // Konfigurer relasjoner her
            builder.Entity<JobListing>()
                .HasOne(j => j.EmployerUser)
                .WithMany()
                .HasForeignKey(j => j.EmployerUserId);
        }
    }
}