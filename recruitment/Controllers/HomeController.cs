using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using recruitment.Data;
using recruitment.Models;
using System.Diagnostics;
using System.Linq;

namespace recruitment.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context; // Assuming you have a DbContext

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
		{
			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

        [HttpGet]
        public IActionResult Search(string searchTitle)
        {
            var jobs = _context.JobListings
                .Where(j => j.Title.Contains(searchTitle) || string.IsNullOrEmpty(searchTitle))
                .ToList();

            return View(jobs); // Assuming you have a view ready to display the list of jobs
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
	}
}
