using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using recruitment.Data;
using recruitment.Models;

namespace recruitment.Controllers
{
    public class JobListingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private string? searchString;

        public JobListingsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: JobListings
        public async Task<IActionResult> Index()
        {
            var jobListings = from j in _context.JobListings.Include(j => j.JobCategory)
                              select j;

            if (!String.IsNullOrEmpty(searchString))
            {
                jobListings = jobListings.Where(s => s.Title.Contains(searchString));
            }

            return View(await jobListings.ToListAsync());
        }
        [HttpGet]
        public async Task<IActionResult> Apply(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Redirect("/Account/Login");
            }

            // Giả sử bạn có một phương thức để lấy CVId từ UserId
            var cv = _context.CVs.FirstOrDefault(cv => cv.UserId == currentUser.Id);
            if (cv == null)
            {
                return RedirectToAction("Create", "CVs"); // Redirect users to create a CV if they don't have one yet
            }

            var application = new UserApplication
            {
                CVId = cv.CVId,
                JobListingId = id,
                ApplicationStatusId = 1 // Giả sử 1 là trạng thái "Đã nộp"
            };

            _context.Add(application);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index"); // Or any notification page you want to display after successful application
        }

        // GET: JobListings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.JobListings == null)
            {
                return NotFound();
            }

            var jobListing = await _context.JobListings
                .Include(j => j.JobCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jobListing == null)
            {
                return NotFound();
            }

            return View(jobListing);
        }

        // GET: JobListings/Create
        public IActionResult Create()
        {
            ViewData["JobCategoryId"] = new SelectList(_context.JobCategories, "JobCategoryId", "CategoryName");
            return View();
        }

        // POST: JobListings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,ApplicationDeadline,JobCategoryId")] JobListing jobListing)
        {
            if (ModelState.IsValid)
            {
                _context.Add(jobListing);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["JobCategoryId"] = new SelectList(_context.JobCategories, "JobCategoryId", "JobCategoryId", jobListing.JobCategoryId);
            return View(jobListing);
        }

        // GET: JobListings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.JobListings == null)
            {
                return NotFound();
            }

            var jobListing = await _context.JobListings.FindAsync(id);
            if (jobListing == null)
            {
                return NotFound();
            }
            ViewData["JobCategoryId"] = new SelectList(_context.JobCategories, "JobCategoryId", "CategoryName", jobListing.JobCategoryId);
            return View(jobListing);
        }

        // POST: JobListings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,ApplicationDeadline,JobCategoryId")] JobListing jobListing)
        {
            if (id != jobListing.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(jobListing);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobListingExists(jobListing.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["JobCategoryId"] = new SelectList(_context.JobCategories, "JobCategoryId", "CategoryName", jobListing.JobCategoryId);
            return View(jobListing);
        }

        // GET: JobListings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.JobListings == null)
            {
                return NotFound();
            }

            var jobListing = await _context.JobListings
                .Include(j => j.JobCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jobListing == null)
            {
                return NotFound();
            }

            return View(jobListing);
        }

        // POST: JobListings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.JobListings == null)
            {
                return Problem("Entity set 'ApplicationDbContext.JobListings'  is null.");
            }
            var jobListing = await _context.JobListings.FindAsync(id);
            if (jobListing != null)
            {
                _context.JobListings.Remove(jobListing);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool JobListingExists(int id)
        {
          return (_context.JobListings?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
