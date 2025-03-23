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
    public class CVsController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public CVsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: CVs
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Problem("You need to login to continue.");
            }

            var userRoles = await _userManager.GetRolesAsync(currentUser);

            // Check if the current user is an Admin or not
            if (userRoles.Contains("Admin"))
            {
            // If you are an Admin, get all CVs
                return _context.CVs != null ?
                    View(await _context.CVs.ToListAsync()) :
                    Problem("Entity set 'ApplicationDbContext.CVs'  is null.");
            }
            else
            {
            // If not Admin, only get that user's CV
                return _context.CVs != null ?
                    View(await _context.CVs.Where(cv => cv.UserId == currentUser.Id).ToListAsync()) :
                    Problem("Entity set 'ApplicationDbContext.CVs'  is null.");
            }
        }


        // GET: CVs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.CVs == null)
            {
                return NotFound();
            }

            var cV = await _context.CVs.FindAsync(id);
            if (cV == null)
            {
                return NotFound();
            }

            return View(cV);
        }

        // GET: CVs/Create
        public IActionResult Create()
        {
            var userId = _userManager.GetUserId(User);
            ViewBag.UserId = userId;
            return View();
        }

        // POST: CVs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CVId,FullName,Experience,Specialization,SelfDescription,UserId,Address,PhoneNumber")] CV cV)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(cV.UserId))
                {
                    cV.UserId = _userManager.GetUserId(User);
                }

                _context.Add(cV);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cV);
        }

        // GET: CVs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.CVs == null)
            {
                return NotFound();
            }

            var cV = await _context.CVs.FindAsync(id);
            if (cV == null)
            {
                return NotFound();
            }
            var cVId = cV.CVId;
            ViewBag.UserId = cV.UserId;
            return View(cV);
        }

        // POST: CVs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CVId,FullName,Experience,Specialization,SelfDescription,UserId,Address,PhoneNumber")] CV cV)
        {
            if (id != cV.CVId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cV);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CVExists(cV.CVId))
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", cV.UserId);
            return View(cV);
        }

        // GET: CVs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.CVs == null)
            {
                return NotFound();
            }
            var cV = await _context.CVs.FindAsync(id);
            if (cV == null)
            {
                return NotFound();
            }

            return View(cV);
        }

        // POST: CVs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.CVs == null)
            {
                return Problem("Entity set 'ApplicationDbContext.CVs'  is null.");
            }
            var cV = await _context.CVs.FindAsync(id);
            if (cV != null)
            {
                _context.CVs.Remove(cV);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CVExists(int id)
        {
          return (_context.CVs?.Any(e => e.CVId == id)).GetValueOrDefault();
        }
    }
}

