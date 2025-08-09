using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Group_11.DAL;
using Group_11.Models;
using static System.Net.Mime.MediaTypeNames;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Group_11.Models.ViewModels;
using Group_11.Utilities;

// TODO: Review Scaffolded Code
namespace Group_11.Controllers
{
    [Authorize]
    public class InterviewsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public InterviewsController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Interviews
        public async Task<IActionResult> Index()
        {
            // query for all applications list
            var interviews = await _context.Interviews
                .Include(i => i.Student)
                .Include(i => i.Interviewer)
                .Include(i => i.Position)
                .ThenInclude(i =>i.Company)
                .ToListAsync();

            // get user id
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // create other userInterview
            IEnumerable<Interview> userInterviews = new List<Interview>();


            // limit query to student's interviews only
            if (User.Identity.IsAuthenticated && User.IsInRole("Student") && userId is not null)
            {

                interviews = interviews
                .Where(a => a.Student != null && a.Student.Id == userId)
                .ToList();
            }

            // limit query to Company interviews only
            if (User.Identity.IsAuthenticated && User.IsInRole("Recruiter") && userId is not null)
            {
                // get recruiter's company
                var company = await _context.Companies.SingleOrDefaultAsync(c => c.Users.Any(u => u.Id == userId));
                Console.WriteLine(company.Name);

                interviews = interviews
                .Where(a => a.Position.Company == company)
                .ToList();
            }

            // get additional list for recruiter/CSO to see own interviews
            if(User.Identity.IsAuthenticated && (User.IsInRole("Recruiter") || User.IsInRole("CSO")) && userId is not null)
            {
                // get recruiter
                var currUser = await _userManager.FindByIdAsync(userId);
                if (currUser == null) { return NotFound(); }

                userInterviews = interviews.Where(i => i.Interviewer == currUser);
            }

            // create view model
            var viewModel = new InterviewIndexViewModel
            {
                AllInterviews = interviews,
                UserInterviews = userInterviews
            };

            return View(viewModel);
        }

        // GET: Interviews/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            // check parameter
            if (id == null)
            {
                return NotFound();
            }

            // find interview where InterviewId == id
            var interview = await _context.Interviews
                .Include(i => i.Student)
                .Include(i => i.Interviewer)
                .Include(i => i.Position)
                .ThenInclude(i => i.Company)
                .FirstOrDefaultAsync(i => i.InterviewId == id);

            // check that interview exists
            if (interview == null)
            {
                return NotFound();
            }

            // return view
            return View(interview);
        }

        // GET: Interviews/Create
        // For Recruiter/CSO to create a new interview slot
        [HttpGet]
        [Authorize(Roles = "Recruiter, CSO")]
        public async Task<IActionResult> Create(int? positionId)
        {
            // check parameter exists
            if (positionId == null)
            {
                return NotFound();
            }

            // query position where PositionId == positionId
            var position = await _context.Positions
                .Include(p => p.Company)
                .FirstOrDefaultAsync(p => p.PositionId == positionId);

            // check position exists
            if (position == null)
            {
                return NotFound();
            }

            // get existing interviews
            var existingInterviews = await _context.Interviews.Select(i => new {
                Date = i.DateTime.ToString("yyyy-MM-dd"),
                Time = i.DateTime.ToString("HH:mm"), 
                Room = i.Room })
                .ToListAsync();
            ViewBag.ExistingInterviews = existingInterviews;

            // create base interview
            var interview = new Interview
            {
                Position = position
            };

            // return view with default interview
            ViewBag.interviewerList = await GetInterviewerList(positionId);
            return View(interview);
        }

        // POST: Interviews/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Recruiter, CSO")]
        public async Task<IActionResult> Create([Bind("InterviewId,Room,DateTime")] Interview interview, int positionId, string InterviewerId)
        {
            // check parameter entry
            if (positionId == 0 || InterviewerId == null)
            {
                return NotFound();
            }

            // get position
            var selectPosition = await _context.Positions
                .Include(p => p.Company)
                .FirstOrDefaultAsync(p => p.PositionId == positionId);
            // get interviewer
            var interviewer = await _userManager.FindByIdAsync(InterviewerId);
            // get existing interviews
            var existingInterviews = await _context.Interviews.Select(i => new {
                Date = i.DateTime.ToString("yyyy-MM-dd"),
                Time = i.DateTime.ToString("HH:mm"),
                Room = i.Room})
                .ToListAsync();

            // check that position and interviewer exists
            if (selectPosition is null)
            {
                ModelState.AddModelError("", "Invalid position.");
                //repopulate and push to view
                interview.Position = selectPosition;
                ViewBag.ExistingInterviews = existingInterviews;
                ViewBag.interviewerList = await GetInterviewerList(positionId);
                return View(interview);
            }
            if(interviewer is null)
            {
                ModelState.AddModelError("", "Invalid interviewer.");
                //repopulate and push to view
                interview.Position = selectPosition;
                ViewBag.ExistingInterviews = existingInterviews;
                ViewBag.interviewerList = await GetInterviewerList(positionId);
                return View(interview);
            }

            // Check if the interview is at least 48 hours after the position deadline
            if (interview.DateTime < selectPosition.Deadline.AddHours(48))
            {
                ModelState.AddModelError("DateTime", "Interview must be at least 48 hours after the application deadline.");
            }

            // Check that interview slot is not already occupied
            bool isSlotTaken = await _context.Interviews.AnyAsync(i =>
                i.DateTime == interview.DateTime && i.Room == interview.Room);
            if (isSlotTaken) 
            {
                ModelState.AddModelError("DateTime", "Slot is already booked.");
            }

            // check for errors, otherwise create interview
            if (ModelState.IsValid)
            {
                // add extra properties
                interview.Position = selectPosition;
                interview.Interviewer = interviewer;

                _context.Add(interview);
                await _context.SaveChangesAsync();

                // push to view
                return RedirectToAction(nameof(Index));
            }
            else
            {
                Console.WriteLine("[DEBUG] ModelState is invalid. Errors:");

                foreach (var modelState in ModelState)
                {
                    var key = modelState.Key;
                    foreach (var error in modelState.Value.Errors)
                    {
                        Console.WriteLine($"[MODEL ERROR] Field: {key}, Error: {error.ErrorMessage}");
                    }
                }
            }

            // return create view if error
            interview.Position = selectPosition;
            ViewBag.ExistingInterviews = existingInterviews;
            ViewBag.interviewerList = await GetInterviewerList(positionId);
            return View(interview);
        }


        // GET: Interviews/Edit/5
        [Authorize(Roles = "Recruiter, CSO")]
        public async Task<IActionResult> Edit(int? id)
        {
            // check id parameter
            if (id == null)
            {
                return NotFound();
            }

            // check interview exists
            var interview = await _context.Interviews
                .Include(i => i.Position)
                .FirstOrDefaultAsync(i => i.InterviewId == id);

            if (interview == null)
            {
                return NotFound();
            }

            // populate viewbag and return view
            var existingInterviews = await _context.Interviews.Select(i => new {
                Date = i.DateTime.ToString("yyyy-MM-dd"),
                Time = i.DateTime.ToString("HH:mm"),
                Room = i.Room
                })
        .       ToListAsync();
            ViewBag.ExistingInterviews = existingInterviews;
            ViewBag.interviewerList = await GetInterviewerList(interview.Position.PositionId);
            return View(interview);
        }

        // POST: Interviews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Recruiter, CSO")]
        public async Task<IActionResult> Edit(int id, [Bind("InterviewId,Room,DateTime")] Interview interview, string? InterviewerId, int? PositionId)
        {
            // check model id
            if (id != interview.InterviewId)
            {
                return NotFound();
            }

            // check position parameter
            if (PositionId == null) { return NotFound();  }

            // check that date time is not already taken
            // Check that interview slot is not already occupied
            bool isSlotTaken = await _context.Interviews.AnyAsync(i =>
                i.DateTime == interview.DateTime && i.Room == interview.Room);
            if (isSlotTaken)
            {
                var slot = await _context.Interviews.Where(i =>
                i.DateTime == interview.DateTime && i.Room == interview.Room).FirstOrDefaultAsync();

                if(slot.InterviewId != interview.InterviewId)
                {
                    // add error to model state
                    ModelState.AddModelError("DateTime", "Slot is already booked.");
                }
            }

            // check model state
            if (ModelState.IsValid)
            {
                try
                {
                    // get and check current interview
                    var interviewToUpdate = await _context.Interviews
                        .Include(i => i.Position)
                        .Include(i => i.Interviewer)
                        .Include(i => i.Student)
                        .FirstOrDefaultAsync(i => i.InterviewId == id);
                    if (interviewToUpdate == null)
                    {
                        return NotFound();
                    }

                    // update fields
                    interviewToUpdate.Room = interview.Room;
                    interviewToUpdate.DateTime = interview.DateTime;

                    // Update interviewer
                    if (InterviewerId != null)
                    {
                        var interviewer = await _userManager.FindByIdAsync(InterviewerId);
                        if (interviewer == null) return NotFound();

                        interviewToUpdate.Interviewer = interviewer;
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InterviewExists(interview.InterviewId))
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

            // re-populate viewbag and return view
            var existingInterviews = await _context.Interviews.Select(i => new {
                Date = i.DateTime.ToString("yyyy-MM-dd"),
                Time = i.DateTime.ToString("HH:mm"),
                Room = i.Room
            })
            .ToListAsync();

            // add back Position
            var position = await _context.Positions
            .FirstOrDefaultAsync(p => p.PositionId == PositionId);
            if (position == null)
            {
                return NotFound();
            }
            interview.Position = position;

            ViewBag.ExistingInterviews = existingInterviews;
            ViewBag.interviewerList = await GetInterviewerList(PositionId);
            return View(interview);
        }

        // GET: Schedule Interview
        [HttpGet]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Schedule(int? positionId)
        {
            // check parameter exists
            if (positionId == null)
            {
                return NotFound();
            }


            var position = await _context.Positions
                .Include(p => p.Company)
                .FirstOrDefaultAsync(p => p.PositionId == positionId);

            // check position exists
            if (position == null)
            {
                return NotFound();
            }

            // call helper & populate Viewbag
            var interviews = await GetInterviewSlots(positionId);
            ViewBag.interviewList = interviews;

            return View(position);
        }

        // POST: Schedule Interview
        [HttpPost]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Schedule(int? positionId, int? interviewId)
        {
            // check parameter exists
            if (positionId == null || interviewId == null)
            {
                return NotFound();
            }

            // query position where PositionId == positionId
            var position = await _context.Positions
                .FirstOrDefaultAsync(p => p.PositionId == positionId);
            // query interview where InterviewId = interviewId
            var interview = await _context.Interviews
                .FirstOrDefaultAsync(i => i.InterviewId == interviewId);

            // check position exists
            if (position == null || interview == null)
            {
                return NotFound();
            }

            // Get and check current user ID from claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var student = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (student == null)
            {
                return NotFound("Student not found.");
            }

            // assign and update interview
            interview.Student = student;
            await _context.SaveChangesAsync();

            // call email helper for messaging
            var fullInterview = await _context.Interviews
                .Include(i => i.Student)
                .Include(i => i.Interviewer)
                .Include(i => i.Position)
                .ThenInclude(i => i.Company)
                .FirstOrDefaultAsync(i => i.InterviewId == interview.InterviewId);
            if (fullInterview == null) { return NotFound(); }
            SendEmail(fullInterview);

            // reroute to Details 
            return RedirectToAction("Details", "Interviews", new { id = interviewId });
        }

        [HttpPost]
        [Authorize(Roles = "Recruiter, CSO")]
        public async Task<IActionResult> AssignStudent(string? StudentId, int? InterviewId)
        {
            // check parameters
            if (InterviewId == null || StudentId == null)
            {
                return NotFound();
            }

            // get interview and student
            var interview = await _context.Interviews.FirstOrDefaultAsync(i => i.InterviewId == InterviewId);
            var student = await _userManager.FindByIdAsync(StudentId);

            if (interview == null || student == null)
            {
                return NotFound();
            }

            // update student
            interview.Student = student; 
            await _context.SaveChangesAsync();

            // call email helper for messaging
            var fullInterview = await _context.Interviews
                .Include(i => i.Student)
                .Include(i => i.Interviewer)
                .Include(i => i.Position)
                .ThenInclude(i => i.Company)
                .FirstOrDefaultAsync(i => i.InterviewId == interview.InterviewId);
            if (fullInterview == null) { return NotFound(); }
            SendEmail(fullInterview);

            // reroute to UserDetails
            return RedirectToAction("UserDetails", "Account", new { id = StudentId });
        }

        /*
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var interview = await _context.Interviews
                .FirstOrDefaultAsync(m => m.InterviewId == id);
            if (interview == null)
            {
                return NotFound();
            }

            return View(interview);
        }

        // POST: Interviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var interview = await _context.Interviews.FindAsync(id);
            if (interview != null)
            {
                _context.Interviews.Remove(interview);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }*/


        //-------------------------------------------------------//
        //Helper methods

        // create select list of interviewers for a given position
        public async Task<SelectList> GetInterviewerList(int? positionId)
        {
            // check parameter exists
            if (positionId == null)
            {
                throw new ArgumentNullException(nameof(positionId));
            }

            // query position where PositionId == positionId
            var position = await _context.Positions
                .Include(p => p.Company)
                .ThenInclude(p => p.Users)
                .FirstOrDefaultAsync(p => p.PositionId == positionId);

            // check position exists
            if (position == null)
            {
                throw new InvalidOperationException("Position not found.");
            }

            // get company
            var company = position.Company;

            // get list of interviewers
            var interviewers = company.Users;

            return new SelectList(interviewers, "Id", "FullName");
        }

        // helper method to create Select List of interview slots
        public async Task<SelectList> GetInterviewSlots(int? positionId)
        {
            // check parameter exists
            if (positionId == null)
            {
                throw new ArgumentNullException(nameof(positionId));
            }

            // query position where PositionId == positionId
            var position = await _context.Positions
                .FirstOrDefaultAsync(p => p.PositionId == positionId);

            // check position exists
            if (position == null)
            {
                throw new InvalidOperationException("Position not found.");
            }

            // get list of interviews
            var interviews = await _context.Interviews
                .Where(i => i.Position == position)
                .Where(i => i.Student == null)
                .ToListAsync();

            if (interviews.Any(i => i == null))
                throw new Exception("One or more interview entries are null.");

            return new SelectList(interviews, "InterviewId", "displayDetails");
        }

        // check that interview exists
        private bool InterviewExists(int id)
        {
            return _context.Interviews.Any(e => e.InterviewId == id);
        }

        // email helper method
        private void SendEmail(Interview interview)
        {
            // create email subject
            string emailSubject = $"Your Interview for {interview.Position.Title} Has been Scheduled";

            // create email body
            string emailBody = $"Congratulations on advancing to the interview Round! Here are the details:" +
                $"\n\nTitle: {interview.Position.Title} | Company: {interview.Position.Company.Name}" +
                $"\n\nDate: {interview.DateTime:dddd, MMM d 'at' h:mm tt}" +
                $"\n\nRoom: {interview.Room}" +
                $"\n\nStudent: {interview?.Student?.FullName ?? "Unknown"}" +
                $"\n\nRecruiter: {interview?.Interviewer?.FullName ?? "Unknown"}";

            // send to student
            string studentAddress = interview.Student.Email;
            if (studentAddress == null)
            {
                throw new InvalidOperationException("Student email address is null.");
            }
            EmailMessaging.SendEmail(emailSubject, emailBody, studentAddress);

            // send to recruiter
            string interviewerAddress = interview.Interviewer.Email;
            if (interviewerAddress == null)
            {
                throw new InvalidOperationException("Interviewer email address is null.");
            }
            EmailMessaging.SendEmail(emailSubject, emailBody, interviewerAddress);
        }
    }
}
