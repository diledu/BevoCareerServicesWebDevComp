using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Group_11.DAL;
using Group_11.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Group_11.Utilities;
using Group_11.ViewModels;
using static Group_11.Models.Application;

namespace Group_11.Controllers
{
    [Authorize]
    public class ApplicationsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly SysDate _sysDate;

        public ApplicationsController(AppDbContext context, SysDate sysDate)
        {
            _context = context;
            _sysDate = sysDate;
        }

        // GET: Applications
        public async Task<IActionResult> Index()
        {
            // query for all applications list
            var applications = await _context.Applications
                .Include(a => a.Student)
                .Include(a => a.Position)
                .ThenInclude(a => a.Company)
                .ToListAsync();

            // get user id
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


            // limit query to student's applications only
            if (User.Identity.IsAuthenticated && User.IsInRole("Student") && userId is not null)
            {

                applications = applications.Where(a => a.Student.Id == userId).ToList();
            }

            return View(applications);
        }

        // GET: Applications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            // get current system date time
            var currDateTime = _sysDate.currDateTime;
            ViewBag.currDateTime = currDateTime;

            // check id parameter
            if (id == null)
            {
                return NotFound();
            }

            // get application w/ nav properties
            var application = await _context.Applications
                .Include (a => a.Student)
                .Include(a => a.Position)
                .ThenInclude(a => a.Company)
                .FirstOrDefaultAsync(m => m.ApplicationId == id);

            // check that application exists
            if (application == null)
            {
                return NotFound();
            }

            // return view
            return View(application);
        }

        // GET: Applications/Create?positionId=#
        [HttpGet]
        public async Task<IActionResult> Create(int positionId)
        {
            var position = await _context.Positions
                .Include(p => p.Majors)
                .Include(p => p.Company)
                .FirstOrDefaultAsync(p => p.PositionId == positionId);

            if (position == null || position.Deadline < DateTime.Now)
                return NotFound("Position not found or deadline passed.");

            var user = await _context.Users
                .Include(u => u.Major)
                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            if (user == null || !User.IsInRole("Student"))
                return Unauthorized();

            bool alreadyApplied = await _context.Applications
                .AnyAsync(a => a.Student.UserName == user.UserName && a.Position.PositionId == positionId);

            if (alreadyApplied)
                return BadRequest("You have already applied to this position.");

            bool majorMatches = position.Majors.Any(m => m.MajorId == user.Major.MajorId);
            if (!majorMatches)
                return BadRequest("Your major does not qualify you for this position.");

            var vm = new ApplicationCreateViewModel
            {
                PositionId = position.PositionId,
                Title = position.Title,
                CompanyName = position.Company?.Name,
                Deadline = position.Deadline,
                Status = "Pending"
            };

            return View(vm);
        }

        // POST: Applications/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApplicationCreateViewModel model)
        {
            var user = await _context.Users
                .Include(u => u.Major)
                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            var position = await _context.Positions
                .Include(p => p.Majors)
                .Include(p => p.Company)
                .FirstOrDefaultAsync(p => p.PositionId == model.PositionId);

            if (user == null || position == null)
                return NotFound();

            bool alreadyApplied = await _context.Applications
                .AnyAsync(a => a.Student.UserName == user.UserName && a.Position.PositionId == model.PositionId);

            if (alreadyApplied)
            {
                ModelState.AddModelError("", "You have already applied to this position.");
                return View(model);
            }

            if (position.Deadline < DateTime.Now ||
                !position.Majors.Any(m => m.MajorId == user.Major.MajorId))
            {
                ModelState.AddModelError("", "You do not meet the application requirements.");
                return View(model);
            }

            var application = new Application
            {
                Student = user,
                Position = position,
                Status = Application.AppStatus.Pending,
                DateTime = DateTime.Now
            };

            if (ModelState.IsValid)
            {
                _context.Applications.Add(application);
                await _context.SaveChangesAsync();

                // ✅ Flash confirmation message
                TempData["SuccessMessage"] = "✅ Your application was submitted successfully!";
                return RedirectToAction("Index", "Positions");
            }
            return View(model);
        }
        
        public async Task<IActionResult> Withdraw(int? id)
        {
            // check id parameter
            if (id == null)
            {
                return NotFound();
            }

            // get application w/ nav properties
            var application = await _context.Applications
                .Include(a => a.Student)
                .Include(a => a.Position)
                .ThenInclude(a => a.Company)
                .FirstOrDefaultAsync(m => m.ApplicationId == id);

            // check that application exists
            if (application == null)
            {
                return NotFound();
            }

            // set status to Withdrawn
            try
            {
                application.Status = Application.AppStatus.Withdrawn;
                // save changes to database
                await _context.SaveChangesAsync();
                TempData["WithdrawMessage"] = "Application has been withdrawn.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ApplicationExists(application.ApplicationId))
                {
                    return NotFound();
                }
                else
                {
                    TempData["WithdrawMessage"] = "Error withdrawing application.";
                    throw;
                }
            }

            // return back to detail view with viewbag
            return RedirectToAction("Details", new { id = application.ApplicationId });
        }

        // helper to check if app exists
        private bool ApplicationExists(int id)
        {
            return _context.Applications.Any(e => e.ApplicationId == id);
        }

        // helper to update app status
        [HttpPost]
        [Authorize(Roles = "Recruiter, CSO")]
        public async Task<IActionResult> UpdateStatus(int? ApplicationId, int? PositionId, AppStatus Status)
        {
            // check parameters
            if(ApplicationId == 0 || PositionId == 0) { return NotFound(); }

            var application = await _context.Applications.FindAsync(ApplicationId);

            // check application exists
            if (application == null)
            {
                return NotFound();
            }

            application.Status = Status;
            _context.Update(application);
            await _context.SaveChangesAsync();

            // Redirect back to the view where the recruiter came from
            return RedirectToAction("Details", "Positions", new { id = PositionId });
        }

    }
}
