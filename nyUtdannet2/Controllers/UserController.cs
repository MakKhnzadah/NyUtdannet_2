using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nyUtdannet2.Models;
using nyUtdannet2.Data;
using System.Threading.Tasks;

namespace nyUtdannet2.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserController> _logger;

        public UserController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context,
            ILogger<UserController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            ViewBag.Roles = roles;

            return View(user);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            ViewBag.Roles = roles;

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            try
            {
   
                var hasJobListings = await _context.JobListings
                    .AnyAsync(j => j.EmployerUserId == id);

                if (hasJobListings)
                {
                    TempData["Error"] = "Kan ikke slette brukeren fordi de har publiserte jobbannonser.";
                    return RedirectToAction(nameof(Delete), new { id });
                }
                
                var hasApplications = await _context.JobApps
                    .AnyAsync(a => a.UserId == id);

                if (hasApplications)
                {
                    TempData["Error"] = "Kan ikke slette brukeren fordi de har aktive jobbsøknader.";
                    return RedirectToAction(nameof(Delete), new { id });
                }

                // Slett favoritter
                var favorites = await _context.Favorites
                    .Where(f => f.UserId == id)
                    .ToListAsync();

                _context.Favorites.RemoveRange(favorites);
                await _context.SaveChangesAsync();

                // Slett brukeren
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Bruker slettet: {UserId}", id);
                    TempData["Success"] = "Brukeren ble slettet.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(user);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Feil ved sletting av bruker: {UserId}", id);
                TempData["Error"] = "Det oppstod en feil ved sletting av brukeren.";
                return RedirectToAction(nameof(Delete), new { id });
            }
        }
    }
}