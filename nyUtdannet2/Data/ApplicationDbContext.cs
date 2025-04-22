// ApplicationDbContext.cs
namespace nyUtdannet2.Data
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using nyUtdannet2.Models;

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Job listings, applications, and favorites
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<JobListing> JobListings { get; set; }
        public DbSet<JobApp> JobApps { get; set; }  
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<EmployerUser> EmployerUsers { get; set; }
        public DbSet<Page> Pages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure relationships
            builder.Entity<JobListing>()
                .HasOne(j => j.EmployerUser)
                .WithMany()
                .HasForeignKey(j => j.EmployerUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<JobApp>()
                .HasOne(app => app.User)
                .WithMany()
                .HasForeignKey(app => app.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<JobApp>()
                .HasOne(app => app.JobListing)
                .WithMany()
                .HasForeignKey(app => app.JobListingId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique constraint for Favorites (1 user can't favorite the same job twice)
            builder.Entity<Favorite>()
                .HasIndex(f => new { f.UserId, f.JobListingId })
                .IsUnique();
            
            builder.Entity<ApplicationUser>()
                .HasMany(u => u.JobApplications)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId);
            
        }
    }
}
