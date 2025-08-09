using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Group_11.DAL;
using Group_11.Models;
using Microsoft.AspNetCore.Authorization;
using Group_11.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace Group_11.Controllers
{
    [Authorize]
    public class PositionsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly SysDate _sysDate;
        private readonly UserManager<AppUser> _userManager;

        public PositionsController(AppDbContext context, SysDate sysDate, UserManager<AppUser> userManager)
        {
            _context = context;
            _sysDate = sysDate;
            _userManager = userManager;
        }

        // GET: Positions
        public async Task<IActionResult> Index()
        {
            // get system date
            var currDateTime = _sysDate.currDateTime;
            var positions = await _context.Positions
            .Include(p => p.Company)
            .Include(p => p.Majors)
            .ToListAsync();

            // limit query to open positions if user is a student
            if(User.IsInRole("Student"))
            {
                positions = positions.Where(p => p.Deadline >= currDateTime).ToList();
            }

            return View(positions);
        }


        // GET: Positions/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            // get current system date time
            var currDateTime = _sysDate.currDateTime;

            if (id == null)
            {
                return NotFound();
            }

            var position = await _context.Positions
                .Include(p => p.Company)
                    .ThenInclude(c => c.Industries)
                .Include(p => p.Majors)
                .FirstOrDefaultAsync(m => m.PositionId == id);

            if (position == null)
            {
                return NotFound();
            }

            if(currDateTime > position.Deadline)
            {
                ViewBag.Applications = await _context.Applications
                    .Include(a => a.Student)
                    .Include(a => a.Position)
                    .ThenInclude(a => a.Company)
                    .Where(a => a.Position.PositionId == position.PositionId)
                    .ToListAsync();

                ViewBag.StatusList = new SelectList(
                    Enum.GetValues(typeof(Application.AppStatus)).Cast<Application.AppStatus>()
                    .Select(s => new SelectListItem
                    {
                        Value = s.ToString(),
                        Text = s.ToString()
                    }),
                    "Value", "Text"
                );
            }

            if(User.IsInRole("Recruiter"))
            {
                // get curruser id and company
                var currUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var company = await _context.Companies.FirstOrDefaultAsync(c => c.Users.Any(u => u.Id == currUserId));
                if(company == null) { return NotFound();  }

                ViewBag.Company = company.CompanyId;
            }


            return View(position);
        }

        // GET: Positions/Create
        public async Task<IActionResult> Create()
        {
            // populate major, state, type list
            var majorList = await _context.Majors.ToListAsync();
            ViewBag.Majors = new MultiSelectList(majorList, "MajorId", "MajorName");
            var stateList = new SelectList(Enum.GetValues(typeof(States)).Cast<States>());
            ViewBag.States = stateList;
            var positionTypeList = new SelectList(Enum.GetValues(typeof(PositionType)).Cast<PositionType>());
            ViewBag.PositionTypes = positionTypeList;

            // for CSO allow them to choose company
            if (User.IsInRole("CSO"))
            {
                var companyList = await _context.Companies.ToListAsync();
                ViewBag.Companies = new SelectList(companyList, "CompanyId", "Name");
            }

            return View();
        }

        // POST: Positions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PositionId,Title,Description,Deadline,Type,City,State")] Position position, int[] Majors, int? Company)
        {
            if (ModelState.IsValid)
            {
                // get list of selected majors
                var selectedMajors = await _context.Majors
                    .Where(m => Majors.Contains(m.MajorId))
                    .ToListAsync();

                // update position Majors
                foreach (var major in selectedMajors)
                {
                    position.Majors.Add(major);
                }

                // default Recruiter to currUser
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null) { return Unauthorized();  }
                var currUser = await _userManager.FindByIdAsync(userId);
                if( currUser == null ) { return NotFound();  }
                position.Recruiter = currUser;

                // assign company
                if(User.IsInRole("CSO") && Company != 0)
                {
                    var company = await _context.Companies.FirstOrDefaultAsync(c => c.CompanyId == Company);
                    if( company == null) { return NotFound();  }
                    position.Company = company;
                }
                else
                {
                    var company = await _context.Companies.FirstOrDefaultAsync(c => c.Users.Any(u => u.Id == userId));
                    if (company == null) { return NotFound(); }
                    position.Company = company;
                }

                _context.Add(position);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"ModelState Error - Field: {state.Key}, Error: {error.ErrorMessage}");
                    }
                }
            }

            // populate lists
            var majorList = await _context.Majors.ToListAsync();
            ViewBag.Majors = new MultiSelectList(majorList, "MajorId", "MajorName");
            var stateList = new SelectList(Enum.GetValues(typeof(States)).Cast<States>());
            ViewBag.States = stateList;
            var positionTypeList = new SelectList(Enum.GetValues(typeof(PositionType)).Cast<PositionType>());
            ViewBag.PositionTypes = positionTypeList;

            // repopulate for CSO to choose company
            if (User.IsInRole("CSO"))
            {
                var companyList = await _context.Companies.ToListAsync();
                ViewBag.Companies = new MultiSelectList(companyList, "CompanyId", "Name");
            }
            return View("Create", position);
        }

        // GET: Positions/Edit/5
        [Authorize(Roles = "CSO,Recruiter")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var position = await _context.Positions
                .Include(p => p.Majors)
                .Include(p => p.Company)
                .Include(p => p.Recruiter)
                .FirstOrDefaultAsync(p => p.PositionId == id);

            if (position == null)
                return NotFound();

            // Dropdowns
            ViewBag.PositionTypes = new SelectList(Enum.GetValues(typeof(PositionType)).Cast<PositionType>());
            ViewBag.States = new SelectList(Enum.GetValues(typeof(States)).Cast<States>());
            ViewBag.MajorOptions = new MultiSelectList(_context.Majors.ToList(), "MajorId", "MajorName", position.Majors.Select(m => m.MajorId));

            return View(position);
        }


        // POST: Positions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "CSO,Recruiter")]
        public async Task<IActionResult> Edit(int id, Position position, List<int> SelectedMajorIds)
        {
            if (id != position.PositionId)
                return NotFound();

            // Remove validation errors for navigation-only fields
            ModelState.Remove("Recruiter");
            ModelState.Remove("Company");
            ModelState.Remove("Majors");

            var existingPosition = await _context.Positions
                .Include(p => p.Company)
                .Include(p => p.Recruiter)
                .Include(p => p.Majors)
                .FirstOrDefaultAsync(p => p.PositionId == id);

            if (existingPosition == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    existingPosition.Title = position.Title;
                    existingPosition.Description = position.Description;
                    existingPosition.Type = position.Type;
                    existingPosition.City = position.City;
                    existingPosition.State = position.State;
                    existingPosition.Deadline = position.Deadline;

                    // Majors
                    existingPosition.Majors = _context.Majors
                        .Where(m => SelectedMajorIds.Contains(m.MajorId))
                        .ToList();

                    _context.Update(existingPosition);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PositionExists(id)) return NotFound();
                    throw;
                }
            }

            // Reload dropdowns on error
            ViewBag.PositionTypes = new SelectList(Enum.GetValues(typeof(PositionType)).Cast<PositionType>());
            ViewBag.States = new SelectList(Enum.GetValues(typeof(States)).Cast<States>());
            ViewBag.MajorOptions = new MultiSelectList(_context.Majors.ToList(), "MajorId", "MajorName", SelectedMajorIds);

            return View(position);
        }


        // GET: Positions/Delete/5
        /*
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var position = await _context.Positions
                .Include(p => p.Company)
                .Include(p => p.Majors)
                .FirstOrDefaultAsync(m => m.PositionId == id);

            if (position == null)
            {
                return NotFound();
            }

            return View(position);
        }

        // POST: Positions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var position = await _context.Positions.FindAsync(id);
            if (position != null)
            {
                _context.Positions.Remove(position);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }*/

        // helper method to check position exists

        private bool PositionExists(int id)
        {
            return _context.Positions.Any(e => e.PositionId == id);
        }

        // GET: Positions/Search
        [HttpGet]
        public async Task<IActionResult> Search()
        {
            var positions = await _context.Positions
                .Include(p => p.Company)
                    .ThenInclude(c => c.Industries)
                .Include(p => p.Majors)
                .ToListAsync();

            var model = new PositionSearchViewModel
            {
                Results = positions
            };

            return View(model);
        }

        // POST: Positions/Search
        [HttpPost]
        public async Task<IActionResult> Search(PositionSearchViewModel model)
        {
            var currDateTime = _sysDate.currDateTime;

            var query = _context.Positions
                .Include(p => p.Company)
                    .ThenInclude(c => c.Industries)
                .Include(p => p.Majors)
                .AsQueryable();

            // Only show open positions for recruiters and students
            if(User.IsInRole("Student") || User.IsInRole("Recruiter"))
            {
                query = query.Where(p => p.Deadline >= currDateTime);
            }

            // filter for company name
            if (!string.IsNullOrEmpty(model.CompanyName))
            {
                query = query.Where(p =>
                    EF.Functions.Like(p.Company.Name ?? "", $"%{model.CompanyName}%"));
            }

            // filter for industry
            if (!string.IsNullOrEmpty(model.IndustryName))
            {
                query = query.Where(p =>
                    p.Company.Industries.Any(i =>
                        EF.Functions.Like(i.IndustryName ?? "", $"%{model.IndustryName}%")));
            }

            // filter for position type
            if (!string.IsNullOrEmpty(model.PositionType))
            {
                if (Enum.TryParse<PositionType>(model.PositionType, out var parsedType))
                {
                    query = query.Where(p => p.Type == parsedType);
                }
            }

            // filter for majors
            if (!string.IsNullOrEmpty(model.MajorName))
            {
                query = query.Where(p =>
                    p.Majors.Any(m =>
                        EF.Functions.Like(m.MajorName ?? "", $"%{model.MajorName}%")));
            }

            // filter for cities
            if (!string.IsNullOrEmpty(model.City))
            {
                query = query.Where(p =>
                    EF.Functions.Like(p.City ?? "", $"%{model.City}%"));
            }

            // filter for states
            if (!string.IsNullOrEmpty(model.State))
            {
                if (Enum.TryParse<States>(model.State, out var parsedState))
                {
                    query = query.Where(p => p.State == parsedState);
                }
            }

            // return detailed search view
            model.Results = await query.ToListAsync();
            return View(model);
        }
    }
}
