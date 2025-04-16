using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;


namespace nyUtdannet2.Models

{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        // Changed from PostalNumber to PostalCode
        [Required]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "Postal code must be a 4-digit code.")]
        public string? PostalCode { get; set; }

        // Separate fields for address details
        [Required]
        [MaxLength(100)]
        public string StreetName { get; set; }

        [Required]
        [MaxLength(10)]
        public string StreetNumber { get; set; }

        [Required]
        [MaxLength(100)]
        public string City { get; set; }

        // Country with default value
        [Required]
        [MaxLength(100)]
        public string Country { get; set; } = "Norway";

        // ... rest of the properties remain the same ...
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime LastOnline { get; set; } = DateTime.UtcNow;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }
    
        public ICollection<JobApp>? JobApplications { get; set; } = new List<JobApp>();
        public string PostalNumber { get; set; }
    }
}