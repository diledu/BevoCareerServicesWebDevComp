using System.Security.Claims;
using System.Threading.Tasks;
using Group_11.DAL;
using Group_11.Models;
using Group_11.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace Project_Template.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        // Create Global Variables
        private readonly AppDbContext _context;
        private readonly SysDate _sysDate;

        public HomeController(AppDbContext context, SysDate sysDate)
        {
            _context = context;
            _sysDate = sysDate;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // get current system date time
            var currDateTime = _sysDate.currDateTime;
            ViewBag.currDateTime = currDateTime;

            if (User.IsInRole("Recruiter"))
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null) { return NotFound(); }
                ;
                // get company
                var company = await _context.Companies
                    .Include(c => c.Users)
                    .Include(c => c.Industries)
                    .FirstOrDefaultAsync(c => c.Users.Any(u => u.Id == userId));
                if (company == null) { return NotFound(); }

                if(company.Email == null || company.Email == "" || company.Description == null || company.Description == "" || !company.Industries.Any())
                {
                    ViewBag.RecruiterNotice = "Thanks for Joining UT Recruiting! Please Update Your Company under 'Manage Company'";
                }
            }

            return View();
        }

        [HttpPost]
        public IActionResult Index(DateTime newDateTime)
        {
            // set time
            _sysDate.SetDateTime(newDateTime);
            // display success message
            TempData["SuccessMessage"] = "Date and time updated successfully!";
            return RedirectToAction("Index");
        }


        [Authorize(Roles = "Recruiter, CSO")]
        [HttpGet]
        public async Task<IActionResult> StudentSearch()
        {
            ViewBag.Majors = GetAllMajors();
            //create db query of all students
            var users = await _context.Users
                .Include(u => u.Major)
                .Where(u => u.Status != Status.Inactive)
                .Where(u => u.Major != null)
                .ToListAsync();
                
            return View(users);
        }

        [Authorize(Roles = "Recruiter, CSO")]
        [HttpPost]
        public IActionResult StudentSearch(string? SearchString, int? Major, int? GraduationYear)
        {
            //create db query of all students
            var query = from s in _context.Users.Include(s => s.Major)
                        where s.Major != null
                        select s;

            if (SearchString is not null)
            {
                // filter First/LastName by search string
                query = query.Where(s => s.FirstName.Contains(SearchString) || s.LastName.Contains(SearchString));
            }
            if(Major is not null)
            {
                // filter major
                query = query.Where(s => s.Major.MajorId == Major);
            }
            if(GraduationYear is not null)
            {
                // filter grad year
                query = query.Where(s => s.GraduationYear == GraduationYear);
            }

            ViewBag.Majors = GetAllMajors();
            return View(query);
        }

        // helper methods
        private SelectList GetAllMajors()
        {
            // Get all majors from the database
            var majors = _context.Majors.ToList();
            // Create a SelectList from the majors
            return new SelectList(majors, "MajorId", "MajorName");
        }
    }
}
