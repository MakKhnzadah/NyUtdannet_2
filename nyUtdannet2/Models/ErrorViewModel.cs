namespace nyUtdannet2.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        
        [Display(Name = "Error Message")]
        public string? ErrorMessage { get; set; }
        
        [Display(Name = "Error Details")]
        public string? ErrorDetails { get; set; }
        
        [Display(Name = "HTTP Status Code")]
        public int? StatusCode { get; set; }
        
        [Display(Name = "Original Path")]
        public string? OriginalPath { get; set; }
        
        [Display(Name = "Occurred At")]
        public DateTime ErrorTime { get; set; } = DateTime.UtcNow;

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        
        // Additional helpful properties
        public string? SupportEmail { get; set; } = "support@yourdomain.com";
        public bool IsDevelopmentEnvironment { get; set; }
        
        // Method to get status code description
        public string GetStatusCodeDescription()
        {
            return StatusCode switch
            {
                400 => "Bad Request",
                401 => "Unauthorized",
                403 => "Forbidden",
                404 => "Not Found",
                500 => "Internal Server Error",
                _ => "An error occurred"
            };
        }
    }
}