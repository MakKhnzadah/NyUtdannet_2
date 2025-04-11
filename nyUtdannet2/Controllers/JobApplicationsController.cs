using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using nyUtdannet2.Models;
using nyUtdannet2.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace nyUtdannet2.Controllers
{
    [Authorize]
    public class JobApplicationsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<JobApplicationsController> _logger;

        public JobApplicationsController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            ILogger<JobApplicationsController> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            if (await _userManager.IsInRoleAsync(user, "Employer"))
            {
                var applications = await _context.JobApps
                    .Where(a => a.JobListing.EmployerUserId == user.Id)
                    .Include(a => a.JobListing)
                    .Include(a => a.User)
                    .OrderByDescending(a => a.SubmittedDate)
                    .ToListAsync();

                return View("Employer/Index", applications);
            }
            else if (await _userManager.IsInRoleAsync(user, "Employee"))
            {
                var applications = await _context.JobApps
                    .Where(a => a.UserId == user.Id)
                    .Include(a => a.JobListing)
                        .ThenInclude(j => j.EmployerUser)
                    .OrderByDescending(a => a.SubmittedDate)
                    .ToListAsync();

                return View("Employee/Index", applications);
            }

            return Forbid();
        }

        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Apply(int id)
        {
            var jobListing = await _context.JobListings
                .FirstOrDefaultAsync(j => j.Id == id && j.IsActive && j.Deadline > DateTime.UtcNow);

            if (jobListing == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }

            var hasApplied = await _context.JobApps
                .AnyAsync(a => a.JobListingId == id && a.UserId == userId);

            if (hasApplied)
            {
                TempData["Message"] = "You have already applied to this position";
                return RedirectToAction(nameof(Index));
            }

            var jobApp = new JobApp
            {
                Title = $"Application for {jobListing.Title}",
                Summary = string.Empty, // Initialiser med tom streng
                Content = string.Empty, // Initialiser med tom streng
                JobListingId = id, // Bruk parameteren 'id' direkte
                UserId = userId,
                Status = ApplicationStatus.Submitted
            };

            return View(jobApp);
        }

        [HttpPost]
        [Authorize(Roles = "Employee")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(JobApp model)
        {
            if (model == null || !ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var userId = _userManager.GetUserId(User) ?? throw new InvalidOperationException("User not found");
                model.UserId = userId;
                model.SubmittedDate = DateTime.UtcNow;
                model.UpdatedDate = DateTime.UtcNow;
                model.Status = ApplicationStatus.Submitted;

                _context.JobApps.Add(model);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Application submitted successfully!";
                return RedirectToAction(nameof(Details), new { id = model.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting job application");
                ModelState.AddModelError("", "An error occurred while submitting your application");
                return View(model);
            }
        }

        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> Details(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }

            var application = await _context.JobApps
                .Include(a => a.User)
                .Include(a => a.JobListing)
                .FirstOrDefaultAsync(a => a.Id == id && a.JobListing.EmployerUserId == userId);

            if (application == null)
            {
                return NotFound();
            }

            return View(application);
        }

        [HttpPost]
        [Authorize(Roles = "Employer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, ApplicationStatus status)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }

            var application = await _context.JobApps
                .Include(a => a.JobListing)
                .FirstOrDefaultAsync(a => a.Id == id && a.JobListing.EmployerUserId == userId);

            if (application == null)
            {
                return NotFound();
            }

            application.Status = status;
            application.UpdatedDate = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
                TempData["Success"] = "Application status updated successfully";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating application status");
                TempData["Error"] = "Failed to update application status";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        [Authorize(Roles = "Employee")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Withdraw(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }

            var application = await _context.JobApps
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

            if (application == null)
            {
                return NotFound();
            }

            if (application.Status != ApplicationStatus.Submitted)
            {
                TempData["Error"] = "Only submitted applications can be withdrawn";
                return RedirectToAction(nameof(Index));
            }

            application.Status = ApplicationStatus.Withdrawn;
            application.UpdatedDate = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
                TempData["Success"] = "Application withdrawn successfully";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error withdrawing application");
                TempData["Error"] = "Failed to withdraw application";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}