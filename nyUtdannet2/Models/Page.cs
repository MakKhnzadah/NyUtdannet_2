using System;
using System.ComponentModel.DataAnnotations;

namespace nyUtdannet2.Models
{
    public class Page
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public required string Title { get; set; }
        
        [Required]
        [StringLength(100)]
        public required string Slug { get; set; }
        
        [Required]
        public required string Content { get; set; }
        
        public string? MetaDescription { get; set; }
        
        public string? MetaKeywords { get; set; }
        
        public bool IsPublished { get; set; } = false;
        
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        public DateTime LastModifiedDate { get; set; } = DateTime.UtcNow;
        
        public string? CreatedById { get; set; }
        
        public string? LastModifiedById { get; set; }
    }
}