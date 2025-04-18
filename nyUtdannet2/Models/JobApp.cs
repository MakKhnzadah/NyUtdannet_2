
namespace nyUtdannet2.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class JobApp
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Summary { get; set; }
        public required string Content { get; set; }
        public DateTime SubmittedDate { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Submitted;
        public string? ResumePath { get; set; }
        public string? CoverLetterPath { get; set; }
    
        // Navigation properties
        public required string UserId { get; set; }
        public virtual ApplicationUser User { get; set; } = null!;
        public required int JobListingId { get; set; }
        public virtual JobListing JobListing { get; set; } = null!;
        
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