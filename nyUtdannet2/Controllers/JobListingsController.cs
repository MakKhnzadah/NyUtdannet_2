using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nyUtdannet2.Models;
using nyUtdannet2.Data;
using System.Security.Claims;

namespace nyUtdannet2.Controllers
{
    [Authorize]
    public class JobListingsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<JobListingsController> _logger;

        public JobListingsController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            ILogger<JobListingsController> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            if (await _userManager.IsInRoleAsync(user, "Employer"))
            {
                var listings = await _context.JobListings
                    .Where(j => j.EmployerUserId == user.Id)
                    .OrderByDescending(j => j.CreatedDate)
                    .ToListAsync();

                return View("Employer/Index", listings);
            }
            else
            {
                var activeListings = await _context.JobListings
                    .Include(j => j.EmployerUser)
                    .Where(j => j.IsActive && j.Deadline > DateTime.UtcNow)
                    .OrderByDescending(j => j.CreatedDate)
                    .ToListAsync();

                return View("Employee/Index", activeListings);
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var listing = await _context.JobListings
                .Include(j => j.EmployerUser)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (listing == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            var viewModel = new JobListingDetailsViewModel
            {
                JobListing = listing,
                HasApplied = await _context.JobApps
                    .AnyAsync(a => a.JobListingId == id && a.UserId == userId),
                IsFavorite = await _context.Favorites
                    .AnyAsync(f => f.JobListingId == id && f.UserId == userId)
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Employer")]
        public IActionResult Create()
        {
            return View(new JobListing
            {
                Title = string.Empty,
                Headline = string.Empty,
                Description = string.Empty,
                Requirements = string.Empty,
                Deadline = DateTime.UtcNow.AddDays(30),
                LocationType = "On-site",
                EmploymentType = "Full-time",
                Country = "Norway",
                EmployerUserId = string.Empty
            });
        }

        [HttpPost]
        [Authorize(Roles = "Employer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(JobListing model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Unauthorized();

                model.EmployerUserId = user.Id;
                model.IsActive = true;
                model.CreatedDate = DateTime.UtcNow;
                model.UpdatedDate = DateTime.UtcNow;

                _context.JobListings.Add(model);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Job listing created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating job listing");
                ModelState.AddModelError("", "An error occurred while creating the job listing");
                return View(model);
            }
        }

        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> Edit(int id)
        {
            var listing = await _context.JobListings.FindAsync(id);
            if (listing == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (listing.EmployerUserId != userId)
            {
                return Forbid();
            }

            return View(listing);
        }

        [HttpPost]
        [Authorize(Roles = "Employer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, JobListing model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var userId = _userManager.GetUserId(User);
                var existingListing = await _context.JobListings.FindAsync(id);

                if (existingListing == null || existingListing.EmployerUserId != userId)
                {
                    return Forbid();
                }

                existingListing.Title = model.Title;
                existingListing.Headline = model.Headline;
                existingListing.Description = model.Description;
                existingListing.Requirements = model.Requirements;
                existingListing.Benefits = model.Benefits;
                existingListing.LocationType = model.LocationType;
                existingListing.City = model.City;
                existingListing.Country = model.Country;
                existingListing.SalaryRange = model.SalaryRange;
                existingListing.EmploymentType = model.EmploymentType;
                existingListing.Deadline = model.Deadline;
                existingListing.IsActive = model.IsActive;
                existingListing.UpdatedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                TempData["Success"] = "Job listing updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating job listing");
                ModelState.AddModelError("", "An error occurred while updating the job listing");
                return View(model);
            }
        }

        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> Delete(int id)
        {
            var listing = await _context.JobListings
                .FirstOrDefaultAsync(j => j.Id == id);

            if (listing == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (listing.EmployerUserId != userId)
            {
                return Forbid();
            }

            return View(listing);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Employer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var listing = await _context.JobListings.FindAsync(id);
                if (listing == null)
                {
                    return NotFound();
                }

                _context.JobListings.Remove(listing);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Job listing deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting job listing");
                TempData["Error"] = "An error occurred while deleting the job listing";
                return RedirectToAction(nameof(Index));
            }
        }

        [Authorize(Roles = "Employee")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleFavorite(int jobListingId)
        {
            var userId = _userManager.GetUserId(User) ?? throw new InvalidOperationException("User not found");
            
            var existingFavorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.JobListingId == jobListingId && f.UserId == userId);

            if (existingFavorite == null)
            {
                _context.Favorites.Add(new Favorite
                {
                    JobListingId = jobListingId,
                    UserId = userId
                });
                TempData["Success"] = "Added to favorites";
            }
            else
            {
                _context.Favorites.Remove(existingFavorite);
                TempData["Success"] = "Removed from favorites";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = jobListingId });
        }

        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Favorites()
        {
            var userId = _userManager.GetUserId(User) ?? throw new InvalidOperationException("User not found");
            
            var favorites = await _context.Favorites
                .Where(f => f.UserId == userId)
                .Include(f => f.JobListing)
                    .ThenInclude(j => j.EmployerUser)
                .ToListAsync();

            return View(favorites);
        }

        [Authorize(Roles = "Employee")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFavorite(int id)
        {
            var userId = _userManager.GetUserId(User) ?? throw new InvalidOperationException("User not found");
            
            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);

            if (favorite != null)
            {
                _context.Favorites.Remove(favorite);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Removed from favorites";
            }

            return RedirectToAction(nameof(Favorites));
        }
    }

    public class JobListingDetailsViewModel
    {
        public required JobListing JobListing { get; set; }
        public bool HasApplied { get; set; }
        public bool IsFavorite { get; set; }
    }
}