namespace nyUtdannet2.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class JobListing
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        [Display(Name = "Headline/Intro")]
        public string Headline { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [Display(Name = "Job Title")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Job Description")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Requirements")]
        [DataType(DataType.MultilineText)]
        public string Requirements { get; set; } = string.Empty;

        [Display(Name = "Benefits")]
        [DataType(DataType.MultilineText)]
        public string? Benefits { get; set; }

        // Location Information
        [MaxLength(100)]
        [Display(Name = "Location Type")]
        public string LocationType { get; set; } = "On-site"; // On-site, Remote, Hybrid

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(100)]
        public string? Country { get; set; } = "Norway";

        // Salary Information
        [Display(Name = "Salary Range")]
        [MaxLength(100)]
        public string? SalaryRange { get; set; }

        // Dates
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Last Updated")]
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

        [Required]
        [Display(Name = "Application Deadline")]
        public DateTime Deadline { get; set; } = DateTime.UtcNow.AddDays(30);

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        // Employment Type
        [MaxLength(50)]
        [Display(Name = "Employment Type")]
        public string EmploymentType { get; set; } = "Full-time"; // Full-time, Part-time, Contract, etc.

        // Relationships
        [Required]
        [ForeignKey("EmployerUser")]
        public string EmployerUserId { get; set; }
        public virtual EmployerUser EmployerUser { get; set; }

        public virtual ICollection<JobApp> JobApplications { get; set; } = new List<JobApp>();
        public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

        // Constructor for initialization
        public JobListing(string headline, string title, string description, string requirements, string employerUserId)
        {
            Headline = headline;
            Title = title;
            Description = description;
            Requirements = requirements;
            EmployerUserId = employerUserId;
            CreatedDate = DateTime.UtcNow;
            UpdatedDate = DateTime.UtcNow;
            Deadline = DateTime.UtcNow.AddDays(30);
        }

        public JobListing() { }
    }
}