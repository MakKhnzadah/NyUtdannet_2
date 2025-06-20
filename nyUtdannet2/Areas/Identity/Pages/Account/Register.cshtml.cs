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
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
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
            public string FirstName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Etternavn er påkrevd")]
            public string LastName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Postnummer er påkrevd")]
            public string PostalCode { get; set; } = string.Empty;

            [Required(ErrorMessage = "Gateadresse er påkrevd")]
            public string StreetName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Husnummer er påkrevd")]
            public string StreetNumber { get; set; } = string.Empty;

            [Required(ErrorMessage = "Poststed er påkrevd")]
            public string City { get; set; } = string.Empty;

            [Required(ErrorMessage = "Fødselsdato er påkrevd")]
            [DataType(DataType.Date)]
            public DateTime DateOfBirth { get; set; } = DateTime.Now.AddYears(-18);

            [Required(ErrorMessage = "Velg type bruker")]
            [Display(Name = "Brukertype")]
            public string UserType { get; set; } = "Employee"; // أو "Employer"

            // For employer
            [Display(Name = "Firmanavn")]
            public string? CompanyName { get; set; }

            [Display(Name = "Firma beskrivelse")]
            public string? CompanyDescription { get; set; }

            [Display(Name = "Nettside")]
            [Url]
            public string? CompanyWebsite { get; set; }
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
                ApplicationUser user;

                if (Input.UserType == "Employer")
                {
                    user = new EmployerUser
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
                        DateOfBirth = Input.DateOfBirth,
                        CompanyName = Input.CompanyName ?? "",
                        CompanyDescription = Input.CompanyDescription,
                        CompanyWebsite = Input.CompanyWebsite
                    };
                }
                else
                {
                    user = new ApplicationUser
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
                }

                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    // Ensure role exists
                    if (!await _roleManager.RoleExistsAsync(Input.UserType))
                        await _roleManager.CreateAsync(new IdentityRole(Input.UserType));

                    //await _userManager.AddToRoleAsync(user, Input.UserType);

                    //await _signInManager.SignInAsync(user, isPersistent: false);
                    //return LocalRedirect(returnUrl);
                    
                    await _userManager.AddToRoleAsync(user, Input.UserType);
                    TempData["RegistrationSuccess"] = "Registrering vellykket! Vennligst logg inn.";
                    return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
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
