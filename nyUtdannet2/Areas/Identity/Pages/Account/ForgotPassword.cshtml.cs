using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using nyUtdannet2.Models;

namespace nyUtdannet2.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ForgotPasswordModel> _logger;

        public ForgotPasswordModel(UserManager<ApplicationUser> userManager, ILogger<ForgotPasswordModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required(ErrorMessage = "E-post er påkrevd")]
            [EmailAddress(ErrorMessage = "Ugyldig e-postadresse")]
            [Display(Name = "E-post")]
            public string Email { get; set; } = string.Empty;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed for security
                    _logger.LogInformation("Password reset attempted for non-existent or unconfirmed email: {Email}", Input.Email);
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                // In a real application, you would send an email here with a password reset link
                // For this demo, we'll just log it and redirect to confirmation
                _logger.LogInformation("Password reset requested for user with email: {Email}", Input.Email);
                
                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
    }
}