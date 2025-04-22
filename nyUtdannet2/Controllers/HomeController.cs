using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nyUtdannet2.Models;
using nyUtdannet2.Data;
using System.Security.Claims;


namespace nyUtdannet2.Controllers
{
    // Remove blanket [Authorize] to allow anonymous access to specific actions
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public HomeController(
            ILogger<HomeController> logger,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
        }
        
        // This action can be accessed without login
        public async Task<IActionResult> Index()
        {
            // Check if user is authenticated
            if (User.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(User);

                if (user != null)
                {
                    if (await _userManager.IsInRoleAsync(user, "Employer"))
                    {
                        return RedirectToAction("EmployerHome");
                    }
                    else if (await _userManager.IsInRoleAsync(user, "Employee"))
                    {
                        return RedirectToAction("EmployeeHome");
                    }
                }
            }

            // For anonymous users or users without specific roles
            var featuredJobs = await _context.JobListings
                .Where(j => j.IsActive && j.Deadline > DateTime.UtcNow)
                .OrderByDescending(j => j.CreatedDate)
                .Take(3)
                .Include(j => j.EmployerUser)
                .ToListAsync();

            return View(featuredJobs);
        }
        
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(user);
        }
        
        // Start av redigere bruker profil
        [Authorize]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Index", "Home");
 
            var vm = new EditProfileViewModel
            {
                Id           = user.Id,
                FirstName    = user.FirstName,
                LastName     = user.LastName,
                DateOfBirth  = user.DateOfBirth,
                StreetName   = user.StreetName,
                StreetNumber = user.StreetNumber,
                PostalCode   = user.PostalCode,
                City         = user.City,
                Country      = user.Country
            };
            return View(vm);
        }
 
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
 
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
                return RedirectToAction("Index", "Home");
 
            user.FirstName    = model.FirstName;
            user.LastName     = model.LastName;
            user.DateOfBirth  = model.DateOfBirth;
            user.StreetName   = model.StreetName;
            user.StreetNumber = model.StreetNumber;
            user.PostalCode   = model.PostalCode;
            user.City         = model.City;
            user.Country      = model.Country;
            user.UpdatedDate  = DateTime.UtcNow;
 
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["Message"] = "Profilen din er oppdatert!";
                return RedirectToAction(nameof(Profile));
            }
            foreach (var err in result.Errors)
                ModelState.AddModelError("", err.Description);
 
            return View(model);
        }
         
        // End av redigere profil

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { 
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                StatusCode = HttpContext.Response.StatusCode
            });
        }

        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> EmployerHome()
        {
            var employerId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(employerId))
            {
                return Unauthorized();
            }

            var employerUser = await _context.EmployerUsers
                .Include(e => e.JobListings)
                .FirstOrDefaultAsync(e => e.Id == employerId);

            if (employerUser == null)
            {
                return NotFound();
            }

            var viewModel = new EmployerDashboardViewModel
            {
                CompanyName = employerUser.CompanyName,
                ActiveJobs = employerUser.JobListings.Count(j => j.IsActive),
                NewApplications = await _context.JobApps
                    .Where(a => a.JobListing.EmployerUserId == employerId && 
                               a.Status == ApplicationStatus.Submitted)
                    .CountAsync(),
                RecentApplications = await _context.JobApps
                    .Where(a => a.JobListing.EmployerUserId == employerId)
                    .OrderByDescending(a => a.SubmittedDate)
                    .Take(5)
                    .Include(a => a.User)
                    .Include(a => a.JobListing)
                    .ToListAsync()
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> EmployeeHome()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var user = await _context.ApplicationUsers
                .Include<ApplicationUser, object>(u => u.JobApplications)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            var viewModel = new EmployeeDashboardViewModel
            {
                FullName = $"{user.FirstName} {user.LastName}",
                PendingApplications = user.JobApplications?.Count(a => 
                    a.Status == ApplicationStatus.Submitted) ?? 0,
                RecentListings = await _context.JobListings
                    .Where(j => j.IsActive && j.Deadline > DateTime.UtcNow)
                    .OrderByDescending(j => j.CreatedDate)
                    .Take(3)
                    .ToListAsync(),
                RecommendedListings = await GetRecommendedJobs(userId)
            };

            return View(viewModel);
        }

        private async Task<List<JobListing>> GetRecommendedJobs(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return new List<JobListing>();
            }

            return await _context.JobListings
                .Where(j => j.IsActive && j.Deadline > DateTime.UtcNow)
                .OrderByDescending(j => j.CreatedDate)
                .Take(3)
                .ToListAsync();
        }
    }

    public class EmployerDashboardViewModel
    {
        public required string CompanyName { get; set; }
        public int ActiveJobs { get; set; }
        public int NewApplications { get; set; }
        public required List<JobApp> RecentApplications { get; set; }
    }

    public class EmployeeDashboardViewModel
    {
        public required string FullName { get; set; }
        public int PendingApplications { get; set; }
        public required List<JobListing> RecentListings { get; set; }
        public required List<JobListing> RecommendedListings { get; set; }
    }
}