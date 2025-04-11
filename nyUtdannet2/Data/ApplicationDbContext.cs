
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using nyUtdannet2.Models;

using nyUtdannet2.Data;

namespace nyUtdannet2.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // --- Domain Entities ---
    public DbSet<JobApp> JobApplications { get; set; }
    public DbSet<JobListing> JobListings { get; set; }
    public DbSet<EmployerUser> EmployerUsers { get; set; }
    public DbSet<Favorite> Favorites { get; set; }

    // --- Fluent API Configurations ---
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Example: Restrict deletion behavior
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

        // Optional: Unique constraint for Favorites (1 user can't favorite the same job twice)
        builder.Entity<Favorite>()
            .HasIndex(f => new { f.UserId, f.JobListingId })
            .IsUnique();
    }
}
