using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Group_11.DAL;
using Group_11.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

//TODO: Review Scaffolded Code
namespace Group_11.Controllers
{
    public class CompaniesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public CompaniesController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Company Index with search query
        public async Task<IActionResult> Index(string? SearchString, int[] Industries)
        {
            // populate ViewBag
            var industries = await _context.Industries.ToListAsync();
            ViewBag.Industries = new MultiSelectList(industries, "IndustryId", "IndustryName", Industries);

            var companies = await _context.Companies
                .Include(c => c.Industries)
                .ToListAsync();

            // filter with search query
            if (!string.IsNullOrEmpty(SearchString))
            {
                companies = companies.Where(c => c.Name.Contains(SearchString) || c.Description.Contains(SearchString)).ToList();
            }

            // Filter by selected industries (if any selected)
            if (Industries != null && Industries.Any())
            {
                companies = companies.Where(c => c.Industries.Any(i => Industries.Contains(i.IndustryId))).ToList();
            }

            return View(companies);
        }

        // GET: Companies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Companies
                .Include(c => c.Users)
                .Include(c => c.Industries)
                .FirstOrDefaultAsync(m => m.CompanyId == id);
            if (company == null)
            {
                return NotFound();
            }

            // validate User is a Recruiter for Company
            if (User.IsInRole("Recruiter"))
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null) { return NotFound(); }
                var isRecruiterForCompany = company.Users.Any(u => u.Id == userId);
                if (isRecruiterForCompany)
                {
                    ViewBag.isRecruiter = true;
                }
            }

            return View(company);
        }

        // GET: Companies/Create
        [Authorize(Roles = "CSO")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Companies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "CSO")]
        public async Task<IActionResult> Create([Bind("CompanyId,Name,Email,Description")] Company company, int[] Industries)
        {
            if (ModelState.IsValid)
            {
                _context.Add(company);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }

        // GET: Companies/Edit/5
        [Authorize(Roles = "Recruiter, CSO")]
        public async Task<IActionResult> Edit(int? id)
        {
            // check parameter
            if (id == null)
            {
                return NotFound();
            }

            // get company
            var company = await _context.Companies
                .Include(c => c.Users)
                .Include(c => c.Industries)
                .FirstOrDefaultAsync(c => c.CompanyId == id);

            // validate User is a Recruiter for Company
            if(User.IsInRole("Recruiter"))
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null) { return NotFound(); }
                var isRecruiterForCompany = company.Users.Any(u => u.Id == userId);
                if (!isRecruiterForCompany)
                {
                    return Unauthorized();
                }
            }

            // check company exists
            if (company == null)
            {
                return NotFound();
            }

            // populate ViewBag and push to POST
            var industries = await _context.Industries.ToListAsync();
            var selectedIndustryIds = company.Industries.Select(i => i.IndustryId);
            ViewBag.Industries = new MultiSelectList(industries, "IndustryId", "IndustryName", selectedIndustryIds);
            return View(company);
        }

        // POST: Companies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Recruiter, CSO")]
        public async Task<IActionResult> Edit(int id, [Bind("CompanyId,Name,Email,Description")] Company company, int[] Industries)
        {
            if (id != company.CompanyId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // get current Company
                    var companyToUpdate = await _context.Companies
                        .Include(c => c.Industries)
                        .FirstOrDefaultAsync(c => c.CompanyId == id);

                    // update Scalar Properties
                    companyToUpdate.Name = company.Name;
                    companyToUpdate.Email = company.Email;
                    companyToUpdate.Description = company.Description;

                    // get list of selected industries
                    var selectedIndustries = await _context.Industries
                        .Where(i => Industries.Contains(i.IndustryId))
                        .ToListAsync();

                    // reset and update industries
                    companyToUpdate.Industries.Clear();

                    foreach (var industry in selectedIndustries)
                    {
                        companyToUpdate.Industries.Add(industry);
                    }

                    if (companyToUpdate == null)
                    {
                        return NotFound();
                    }

                    if (Industries != null)
                    {
                        foreach (int IndustryId in Industries)
                        {
                        }
                    }
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyExists(company.CompanyId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Companies", new { id = company.CompanyId });
            }


            // add back Industries
            var industryList = await _context.Industries
                .Where(i => i.Companies.Any(c => c.CompanyId == id))
                .ToListAsync();
            company.Industries = industryList;

                        // populate ViewBag and push to POST
            var industries = await _context.Industries.ToListAsync();
            var selectedIndustryIds = company.Industries.Select(i => i.IndustryId);
            ViewBag.Industries = new MultiSelectList(industries, "IndustryId", "IndustryName");

            return View(company);
        }

        // GET: Companies/Delete/5
        /*
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Companies
                .FirstOrDefaultAsync(m => m.CompanyId == id);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var company = await _context.Companies.FindAsync(id);
            if (company != null)
            {
                _context.Companies.Remove(company);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }*/

        //************************************************
        // Helper to check if company exists
        private bool CompanyExists(int id)
        {
            return _context.Companies.Any(e => e.CompanyId == id);
        }

        // Helper to handle Recruiter routing
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> RecruiterCompanyRouting()
        {
            // get recruiter Id
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if( userId == null) { return Unauthorized(); };
            // get company
            var company = await _context.Companies
                .Include(c => c.Users)
                .FirstOrDefaultAsync(c => c.Users.Any(u => u.Id == userId));
            if(company == null) { return NotFound();  }

            return RedirectToAction("Details", "Companies", new { id = company.CompanyId });
        }
    }
}
