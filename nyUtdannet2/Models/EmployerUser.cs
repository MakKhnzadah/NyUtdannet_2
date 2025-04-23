using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace nyUtdannet2.Models
{
    public class EmployerUser : ApplicationUser
    {
        [Required]
        [MaxLength(200)]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }

        [MaxLength(500)]
        [Display(Name = "Company Description")]
        public string? CompanyDescription { get; set; }

        [MaxLength(200)]
        [Display(Name = "Company Website")]
        [Url(ErrorMessage = "Please enter a valid URL")]
        public string? CompanyWebsite { get; set; }

        [MaxLength(200)]
        [Display(Name = "Company Logo URL")]
        [Url(ErrorMessage = "Please enter a valid URL")]
        public string? CompanyLogoUrl { get; set; }
        
        public ICollection<JobListing> JobListings { get; set; } = new List<JobListing>();
    }
}