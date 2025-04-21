using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nyUtdannet2.Data;
using nyUtdannet2.Models;
using System;
using System.Threading.Tasks;

namespace nyUtdannet2.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class PageController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PageController> _logger;

        public PageController(ApplicationDbContext context, ILogger<PageController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var pages = await _context.Pages.ToListAsync();
            return View(pages);
        }

        public async Task<IActionResult> Details(int id)
        {
            var page = await _context.Pages.FindAsync(id);

            if (page == null)
            {
                return NotFound();
            }

            return View(page);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Page page)
        {
            if (ModelState.IsValid)
            {
                page.CreatedDate = DateTime.UtcNow;
                page.LastModifiedDate = DateTime.UtcNow;
                
                _context.Pages.Add(page);
                await _context.SaveChangesAsync();
                
                TempData["Success"] = "Siden ble opprettet.";
                return RedirectToAction(nameof(Index));
            }
            
            return View(page);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var page = await _context.Pages.FindAsync(id);
            
            if (page == null)
            {
                return NotFound();
            }
            
            return View(page);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Page page)
        {
            if (id != page.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingPage = await _context.Pages.FindAsync(id);
                    
                    if (existingPage == null)
                    {
                        return NotFound();
                    }
                    
                    existingPage.Title = page.Title;
                    existingPage.Slug = page.Slug;
                    existingPage.Content = page.Content;
                    existingPage.IsPublished = page.IsPublished;
                    existingPage.LastModifiedDate = DateTime.UtcNow;
                    
                    await _context.SaveChangesAsync();
                    
                    TempData["Success"] = "Siden ble oppdatert.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError(ex, "Feil ved oppdatering av side: {PageId}", id);
                    
                    if (!_context.Pages.Any(e => e.Id == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Feil ved oppdatering av side: {PageId}", id);
                    ModelState.AddModelError(string.Empty, "Det oppstod en feil ved oppdatering av siden.");
                }
            }
            
            return View(page);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var page = await _context.Pages.FindAsync(id);
            
            if (page == null)
            {
                return NotFound();
            }
            
            return View(page);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var page = await _context.Pages.FindAsync(id);
            
            if (page == null)
            {
                return NotFound();
            }
            
            _context.Pages.Remove(page);
            await _context.SaveChangesAsync();
            
            TempData["Success"] = "Siden ble slettet.";
            return RedirectToAction(nameof(Index));
        }
    }
}