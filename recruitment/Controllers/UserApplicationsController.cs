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
    public class UserApplicationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public UserApplicationsController(ApplicationDbContext context,UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: UserApplications
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Redirect("/Account/Login");
            }

            var userId = currentUser.Id;

            // Get a list of IDs of Job Listings that the current user has created
            try
            {


                // Filter UserApplications based on the ID list of JobListings 
                var applicationDbContext = _context.UserApplications
                    .Include(u => u.ApplicationStatus)
                    .Include(u => u.CV)
                    .Include(u => u.JobListing);// Lọc UserApplications dựa trên JobListingId
                return View(await applicationDbContext.ToListAsync());
            }
            catch (Exception ex)
            {
                return Redirect("/Home");
            }
           
        }
        public async Task<IActionResult> MyApplication()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Redirect("/Account/Login");
            }

            var userId = currentUser.Id;

            // Get a list of IDs of Job Listings that the current user has created

            // Filter UserApplications based on the ID list of JobListings
            var applicationDbContext = _context.UserApplications
                .Include(u => u.ApplicationStatus)
                .Include(u => u.CV)
                .Include(u => u.JobListing)
                .Where(u => u.CV.UserId == userId);
            ; // Lọc UserApplications dựa trên JobListingId

            return View(await applicationDbContext.ToListAsync());
        }

        // GET: UserApplications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.UserApplications == null)
            {
                return NotFound();
            }

            var userApplication = await _context.UserApplications
                .Include(u => u.ApplicationStatus)
                .Include(u => u.CV)
                .Include(u => u.JobListing)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userApplication == null)
            {
                return NotFound();
            }

            return View(userApplication);
        }

        // GET: UserApplications/Create
        public IActionResult Create()
        {
            ViewData["ApplicationStatusId"] = new SelectList(_context.ApplicationStatuses, "ApplicationStatusId", "ApplicationStatusId");
            ViewData["CVId"] = new SelectList(_context.CVs, "Id", "Id");
            ViewData["JobListingId"] = new SelectList(_context.JobListings, "Id", "Id");
            return View();
        }

        // POST: UserApplications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CVId,JobListingId,ApplicationStatusId")] UserApplication userApplication)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userApplication);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationStatusId"] = new SelectList(_context.ApplicationStatuses, "ApplicationStatusId", "ApplicationStatusId", userApplication.ApplicationStatusId);
            ViewData["CVId"] = new SelectList(_context.CVs, "Id", "Id", userApplication.CVId);
            ViewData["JobListingId"] = new SelectList(_context.JobListings, "Id", "Id", userApplication.JobListingId);
            return View(userApplication);
        }

        // GET: UserApplications/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.UserApplications == null)
            {
                return NotFound();
            }

            var userApplication = await _context.UserApplications.FindAsync(id);
            if (userApplication == null)
            {
                return NotFound();
            }
            ViewData["ApplicationStatusId"] = new SelectList(_context.ApplicationStatuses, "ApplicationStatusId", "StatusName", userApplication.ApplicationStatusId);
            return View(userApplication);
        }

        // POST: UserApplications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ApplicationStatusId")] UserApplication userApplicationForm)
        {
            if (id != userApplicationForm.Id)
            {
                return NotFound();
            }

            var userApplicationDb = await _context.UserApplications.FindAsync(id);
            if (userApplicationDb == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    userApplicationDb.ApplicationStatusId = userApplicationForm.ApplicationStatusId; // Just update ApplicationStatusId
                    _context.Update(userApplicationDb);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserApplicationExists(userApplicationDb.Id))
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
            ViewData["ApplicationStatusId"] = new SelectList(_context.ApplicationStatuses, "ApplicationStatusId", "ApplicationStatusId", userApplicationForm.ApplicationStatusId);
            return View(userApplicationForm);
        }


        // GET: UserApplications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.UserApplications == null)
            {
                return NotFound();
            }

            var userApplication = await _context.UserApplications
                .Include(u => u.ApplicationStatus)
                .Include(u => u.CV)
                .Include(u => u.JobListing)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userApplication == null)
            {
                return NotFound();
            }

            return View(userApplication);
        }

        // POST: UserApplications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.UserApplications == null)
            {
                return Problem("Entity set 'ApplicationDbContext.UserApplications'  is null.");
            }
            var userApplication = await _context.UserApplications.FindAsync(id);
            if (userApplication != null)
            {
                _context.UserApplications.Remove(userApplication);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserApplicationExists(int id)
        {
          return (_context.UserApplications?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
