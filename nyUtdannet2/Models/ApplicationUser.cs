using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace nyUtdannet2.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required, MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required, Display(Name = "PostalCode")]
        public string PostalCode { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string StreetName { get; set; } = string.Empty;

        [Required, MaxLength(10)]
        public string StreetNumber { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Country { get; set; } = "Norway";

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
        public DateTime LastOnline { get; set; } = DateTime.UtcNow;

        [Required, DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; } = DateTime.Now.AddYears(-18); 
        public virtual ICollection<JobApp> JobApplications { get; set; } = new List<JobApp>();

    }
}