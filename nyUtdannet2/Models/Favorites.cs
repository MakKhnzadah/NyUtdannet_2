namespace nyUtdannet2.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Favorite
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } 

        public virtual ApplicationUser User { get; set; } 

        [Required]
        public int JobListingId { get; set; }  

        public virtual JobListing JobListing { get; set; } 
    }
}