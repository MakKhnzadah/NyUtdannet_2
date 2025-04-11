namespace nyUtdannet2.Models;

public class Favorite
{
    public int Id { get; set; }

    // Foreign key to user (can be either ApplicationUser or EmployerUser)
    public string UserId { get; set; }

    // Foreign key to job listing
    public int JobListingId { get; set; }

    // Navigation properties
    public virtual ApplicationUser User { get; set; }
    public virtual JobListing JobListing { get; set; }

    // Timestamp for when the favorite was created
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}