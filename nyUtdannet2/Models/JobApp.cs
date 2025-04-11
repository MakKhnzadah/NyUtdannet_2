
namespace nyUtdannet2.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class JobApp
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Job Title")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        [Display(Name = "Short Summary")]
        public string Summary { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Application Content")]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; } = string.Empty;

        // Status with more detailed options
        [Required]
        [Display(Name = "Application Status")]
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Submitted;

        // Timestamps
        [Display(Name = "Date Submitted")]
        public DateTime SubmittedDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Last Updated")]
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

        // Documents (optional)
        [MaxLength(255)]
        [Display(Name = "Resume/CV")]
        public string? ResumePath { get; set; }

        [MaxLength(255)]
        [Display(Name = "Cover Letter")]
        public string? CoverLetterPath { get; set; }

        // Relationships
        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        [Required]
        [ForeignKey("JobListing")]
        public int JobListingId { get; set; }
        public virtual JobListing JobListing { get; set; }

        // Constructor for initialization
        public JobApp(string title, string summary, string content, string userId, int jobListingId)
        {
            Title = title;
            Summary = summary;
            Content = content;
            UserId = userId;
            JobListingId = jobListingId;
            Status = ApplicationStatus.Submitted;
            SubmittedDate = DateTime.UtcNow;
            UpdatedDate = DateTime.UtcNow;
        }

        public JobApp() { }
    }

    public enum ApplicationStatus
    {
        Submitted,      // Application submitted by candidate
        UnderReview,    // Employer is reviewing
        Interviewing,   // In interview process
        Offered,        // Job offer extended
        Accepted,       // Offer accepted
        Rejected,       // Application rejected
        Withdrawn       // Candidate withdrew
        ,
        Pending
    }
}