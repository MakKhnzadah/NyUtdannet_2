namespace nyUtdannet2.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Favorite
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }  // Reference to the user

        public virtual ApplicationUser User { get; set; }  // Navigation property to ApplicationUser

        [Required]
        public int JobListingId { get; set; }  // Reference to the JobListing

        public virtual JobListing JobListing { get; set; }  // Navigation property to JobListing
    }
}