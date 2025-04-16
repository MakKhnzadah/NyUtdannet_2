namespace nyUtdannet2.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class JobListing
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Headline { get; set; }
        public required string Description { get; set; }
        public required string Requirements { get; set; }
        public string? Benefits { get; set; }
        public required string LocationType { get; set; } = "On-site";
        public string? City { get; set; }
        public string Country { get; set; } = "Norway";
        public string? SalaryRange { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
        public required DateTime Deadline { get; set; } = DateTime.UtcNow.AddDays(30);
        public bool IsActive { get; set; } = true;
        public required string EmploymentType { get; set; } = "Full-time";
    
        // Navigation properties
        public required string EmployerUserId { get; set; }
        public virtual EmployerUser EmployerUser { get; set; } = null!;
        public virtual ICollection<JobApp> JobApplications { get; set; } = new List<JobApp>();
        public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    }
}