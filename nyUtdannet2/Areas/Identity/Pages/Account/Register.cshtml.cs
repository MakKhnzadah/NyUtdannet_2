// Areas/Identity/Pages/Account/Register.cshtml.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using nyUtdannet2.Models;
using System.ComponentModel.DataAnnotations;

namespace nyUtdannet2.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public string? ReturnUrl { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "E-post er påkrevd")]
            [EmailAddress(ErrorMessage = "Ugyldig e-postadresse")]
            [Display(Name = "E-post")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Passord er påkrevd")]
            [StringLength(100, MinimumLength = 6, ErrorMessage = "Passord må være minst 6 tegn")]
            [DataType(DataType.Password)]
            [Display(Name = "Passord")]
            public string Password { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Display(Name = "Bekreft passord")]
            [Compare("Password", ErrorMessage = "Passordene stemmer ikke overens")]
            public string ConfirmPassword { get; set; } = string.Empty;

            [Required(ErrorMessage = "Fornavn er påkrevd")]
            [Display(Name = "Fornavn")]
            public string FirstName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Etternavn er påkrevd")]
            [Display(Name = "Etternavn")]
            public string LastName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Postnummer er påkrevd")]
            [Display(Name = "Postnummer")]
            public string PostalCode { get; set; } = string.Empty;

            [Required(ErrorMessage = "Gateadresse er påkrevd")]
            [Display(Name = "Gateadresse")]
            public string StreetName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Husnummer er påkrevd")]
            [Display(Name = "Husnummer")]
            public string StreetNumber { get; set; } = string.Empty;

            [Required(ErrorMessage = "Poststed er påkrevd")]
            [Display(Name = "Poststed")]
            public string City { get; set; } = string.Empty;

            [Required(ErrorMessage = "Fødselsdato er påkrevd")]
            [Display(Name = "Fødselsdato")]
            [DataType(DataType.Date)]
            public DateTime DateOfBirth { get; set; } = DateTime.Now.AddYears(-18);
        }

        public void OnGet(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    PostalCode = Input.PostalCode,
                    StreetName = Input.StreetName,
                    StreetNumber = Input.StreetNumber,
                    City = Input.City,
                    Country = "Norway",
                    DateOfBirth = Input.DateOfBirth
                };

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }
    }
}