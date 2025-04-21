using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using nyUtdannet2.Models;
using nyUtdannet2.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;

namespace nyUtdannet2.Controllers
{
    [Authorize]
    public class JobApplicationsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<JobApplicationsController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public JobApplicationsController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            ILogger<JobApplicationsController> logger,
            IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
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

                return View("EmployerJobApplications", applications);
            }
            else if (await _userManager.IsInRoleAsync(user, "Employee"))
            {
                var applications = await _context.JobApps
                    .Where(a => a.UserId == user.Id)
                    .Include(a => a.JobListing)
                        .ThenInclude(j => j.EmployerUser)
                    .OrderByDescending(a => a.SubmittedDate)
                    .ToListAsync();

                return View("EmployeeJobApplications", applications);
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

        [HttpPost]
        [Authorize(Roles = "Employee")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApplyWithFiles(JobApp model, IFormFile resumeFile, IFormFile coverLetterFile)
        {
            if (!ModelState.IsValid)
            {
                return View("Apply", model);
            }

            try
            {
                var userId = _userManager.GetUserId(User) ?? throw new InvalidOperationException("User not found");
                model.UserId = userId;
                model.SubmittedDate = DateTime.UtcNow;
                model.UpdatedDate = DateTime.UtcNow;
                model.Status = ApplicationStatus.Submitted;

                // Handle resume upload
                if (resumeFile != null && resumeFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "resumes");
                    Directory.CreateDirectory(uploadsFolder); // Make sure directory exists
                    
                    var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(resumeFile.FileName)}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await resumeFile.CopyToAsync(stream);
                    }
                    
                    model.ResumePath = $"/uploads/resumes/{uniqueFileName}";
                }

                // Handle cover letter upload
                if (coverLetterFile != null && coverLetterFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "coverletters");
                    Directory.CreateDirectory(uploadsFolder); // Make sure directory exists
                    
                    var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(coverLetterFile.FileName)}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await coverLetterFile.CopyToAsync(stream);
                    }
                    
                    model.CoverLetterPath = $"/uploads/coverletters/{uniqueFileName}";
                }

                _context.JobApps.Add(model);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Application submitted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting job application with files");
                ModelState.AddModelError("", "An error occurred while submitting your application");
                return View("Apply", model);
            }
        }

        public async Task<IActionResult> DownloadFile(int applicationId, string fileType)
        {
            // Check permissions - employers can download files for applications to their jobs
            // and employees can download their own files
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();
            
            var application = await _context.JobApps
                .Include(a => a.JobListing)
                .FirstOrDefaultAsync(a => a.Id == applicationId);
                
            if (application == null) return NotFound();
            
            bool isAuthorized = false;
            
            if (await _userManager.IsInRoleAsync(user, "Employer"))
            {
                isAuthorized = application.JobListing.EmployerUserId == user.Id;
            }
            else if (await _userManager.IsInRoleAsync(user, "Employee"))
            {
                isAuthorized = application.UserId == user.Id;
            }
            
            if (!isAuthorized) return Forbid();
            
            // Determine which file to download
            string filePath = null;
            string contentType = "application/octet-stream";
            string fileName = "";
            
            if (fileType.Equals("resume", StringComparison.OrdinalIgnoreCase))
            {
                filePath = Path.Combine(_webHostEnvironment.WebRootPath, application.ResumePath.TrimStart('/'));
                fileName = $"Resume-{application.Id}.pdf";
                contentType = "application/pdf";
            }
            else if (fileType.Equals("coverletter", StringComparison.OrdinalIgnoreCase))
            {
                filePath = Path.Combine(_webHostEnvironment.WebRootPath, application.CoverLetterPath.TrimStart('/'));
                fileName = $"CoverLetter-{application.Id}.pdf";
                contentType = "application/pdf";
            }
            
            if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
            {
                return NotFound();
            }
            
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, contentType, fileName);
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

        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> SearchCandidates()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();
            
            // You could pre-populate with data if needed, for now just showing the search interface
            return View("CandidateSearch");
        }
        
        [Authorize(Roles = "Employer")]
        [HttpGet("JobApplications/SearchCandidatesResults")]
        public async Task<IActionResult> SearchCandidatesResults(string skills, string status)
        {
            // Existing search implementation moved to a separate route
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();
            
            // Base query - get applications for employer's job listings
            var query = _context.JobApps
                .Include(a => a.User)
                .Include(a => a.JobListing)
                .Where(a => a.JobListing.EmployerUserId == userId);
            
            // Filter by status if specified
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<ApplicationStatus>(status, true, out var statusEnum))
            {
                query = query.Where(a => a.Status == statusEnum);
            }
            
            // Get the applications
            var applications = await query.OrderByDescending(a => a.SubmittedDate).ToListAsync();
            
            // If skills are specified, filter the already retrieved applications
            // This is done in memory because we need to search through the content and summary fields
            if (!string.IsNullOrEmpty(skills))
            {
                // Split skills into individual terms for more precise matching
                var skillTerms = skills.ToLower().Split(new[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                
                applications = applications
                    .Where(a => 
                        skillTerms.Any(term => 
                            (a.Summary?.ToLower().Contains(term) ?? false) || 
                            (a.Content?.ToLower().Contains(term) ?? false) ||
                            (a.Title?.ToLower().Contains(term) ?? false))
                    )
                    .ToList();
            }
            
            ViewBag.Skills = skills;
            ViewBag.Status = status;
            
            return View("CandidateSearchResults", applications);
        }
    }
}