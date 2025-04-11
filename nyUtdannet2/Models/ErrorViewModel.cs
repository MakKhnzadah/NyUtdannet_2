using System;
using System.ComponentModel.DataAnnotations;

namespace nyUtdannet2.Models
{
    public class ErrorViewModel
    {
        [Display(Name = "Request ID")]
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
        public string? SupportEmail { get; set; } = "support@yourdomain.com";
        public bool IsDevelopmentEnvironment { get; set; }
    }
}