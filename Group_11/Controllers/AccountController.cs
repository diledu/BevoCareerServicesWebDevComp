using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using Group_11.DAL;
using Group_11.Utilities;
using System;
using Group_11.Models.ViewModels;
using Group_11.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Group_11.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly PasswordValidator<AppUser> _passwordValidator;
        private readonly AppDbContext _context;

        public AccountController(AppDbContext appDbContext, UserManager<AppUser> userManager, SignInManager<AppUser> signIn)
        {
            _context = appDbContext;
            _userManager = userManager;
            _signInManager = signIn;
            //user manager only has one password validator
            _passwordValidator = (PasswordValidator<AppUser>)userManager.PasswordValidators.FirstOrDefault();
        }

        // GET: /Account/Register
        [AllowAnonymous]
        public IActionResult Register(string? roleType)
        {
            // instantiate model with roleType
            var tempModel = new RegisterViewModel { RoleType = roleType };

            // populate ViewBag
            ViewBag.Majors = GetAllMajors();
            ViewBag.Companies = GetAllCompanies();

            return View(tempModel);
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel rvm)
        {
            //if registration data is valid, create a new user on the database
            if (ModelState.IsValid == false)
            {
                //this is the sad path - something went wrong, 
                //return the user to the register page to try again
                return View(rvm);
            }

            AddUserModel aum = new AddUserModel();

            // Maps the RegisterViewModel to the AppUser domain model
            // Check the different specified roles of user
            if (rvm.RoleType == "Student")
            {
                Major selectMajor = _context.Majors.FirstOrDefault(m => m.MajorId == rvm.Major);

                AppUser newUser = new AppUser
                {
                    UserName = rvm.Email,
                    Email = rvm.Email,
                    PhoneNumber = rvm.PhoneNumber,

                    FirstName = rvm.FirstName,
                    LastName = rvm.LastName,
                    MiddleInitial = rvm.MiddleInitial,
                    Birthday = rvm.Birthday,
                    SSN = rvm.SSN,
                    Street = rvm.Street,
                    City = rvm.City,
                    Zip = rvm.Zip,
                    State = rvm.State,
                    Major = selectMajor,
                    GPA = rvm.GPA,
                    GraduationYear = rvm.GraduationYear,
                    PositionType = rvm.PositionType,
                    Status = Status.Active
                };

                //create AddUserModel
                aum = new AddUserModel()
                {
                    User = newUser,
                    Password = rvm.Password,
                    RoleName = "Student"
                };
            }
            else if (rvm.RoleType == "Recruiter")
            {
                Company selectCompany = _context.Companies.FirstOrDefault(c => c.CompanyId == rvm.Company);

                AppUser newUser = new AppUser
                {
                    UserName = rvm.Email,
                    Email = rvm.Email,
                    PhoneNumber = rvm.PhoneNumber,

                    FirstName = rvm.FirstName,
                    LastName = rvm.LastName,
                    MiddleInitial = rvm.MiddleInitial,
                    Company = selectCompany,
                    Status = Status.Active
                };

                //create AddUserModel
                aum = new AddUserModel()
                {
                    User = newUser,
                    Password = rvm.Password,
                    RoleName = "Recruiter"
                };
            }
            else
            {
                // Create CSO
                AppUser newUser = new AppUser
                {
                    UserName = rvm.Email,
                    Email = rvm.Email,
                    PhoneNumber = rvm.PhoneNumber,

                    FirstName = rvm.FirstName,
                    LastName = rvm.LastName,
                    MiddleInitial = rvm.MiddleInitial,
                    Status = Status.Active
                };

                //create AddUserModel
                aum = new AddUserModel()
                {
                    User = newUser,
                    Password = rvm.Password,
                    RoleName = "CSO"
                };
            }

            //This code uses the AddUser utility to create a new user with the specified password
            IdentityResult result = await Utilities.AddUser.AddUserWithRoleAsync(aum, _userManager, _context);

            if (result.Succeeded) //everything is okay
            {
                // sign user in & push to Home Index if not already logged in
                if (!User.Identity.IsAuthenticated)
                {
                    Microsoft.AspNetCore.Identity.SignInResult result2 = await _signInManager.PasswordSignInAsync(rvm.Email, rvm.Password, false, lockoutOnFailure: false);
                    //Send the user to the home page
                    return RedirectToAction("Index", "Home");
                }

                // default routing for CSO creating new account
                return RedirectToAction("Index", "RoleAdmin");
            }
            else  //the add user operation didn't work, and we need to show an error message
            {
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                //send user back to page with errors
                return View(rvm);
            }
        }

        // GET: /Account/Login
        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated) //user has been redirected here from a page they're not authorized to see
            {
                return View("Error", new string[] { "Access Denied" });
            }
            _signInManager.SignOutAsync(); //this removes any old cookies hanging around
            ViewBag.ReturnUrl = returnUrl; //pass along the page the user should go back to
            return View();
        }

        // POST: /Account/Login
        //NOTE: for future projects, change string returnUrl to string?
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel lvm, string? returnUrl)
        {
            //if user forgot to include user name or password,
            //send them back to the login page to try again
            if (ModelState.IsValid == false)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
                return View(lvm);
            }

            //attempt to sign the user in using the SignInManager
            Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(lvm.Email.ToUpper(), lvm.Password, lvm.RememberMe, lockoutOnFailure: false);

            //if the login worked, take the user to either the url
            //they requested OR the homepage if there isn't a specific url
            if (result.Succeeded)
            {
                //check that user status isnt inactive
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if( userId == null ) { return Unauthorized();  }
                var currUser = await _userManager.FindByIdAsync(userId);
                if(currUser == null) { return Unauthorized();  }

                if(currUser.Status == Status.Inactive)
                {
                    ModelState.AddModelError("", "Account has been deactivated. Please contact support.");
                    await _signInManager.SignOutAsync();
                    return Unauthorized();
                }

                //return ?? "/" means if returnUrl is null, substitute "/" (home)
                return Redirect(returnUrl ?? "/");
            }
            else //log in was not successful
            {
                //add an error to the model to show invalid attempt
                ModelState.AddModelError("", "Invalid login attempt.");
                //send user back to login page to try again
                Console.WriteLine($"Login failed: {result}");
                return View(lvm);
            }
        }

        public IActionResult AccessDenied()
        {
            return View("Error", new string[] { "You are not authorized for this resource" });
        }

        //GET: Account/Index
        public IActionResult Index()
        {
            //get user info
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (currentUserId == null) { return NotFound();  }

            // get user
            AppUser user = user = _context.Users
                .Include(u => u.Major)
                .Include(u => u.Company)
                .FirstOrDefault(u => u.Id == currentUserId);

            if (user == null) { return NotFound();  }

            //send data to the view
            return View(user);
        }

        // edit self profile
        //GET: Account/Edit 
        [HttpGet]
        public async Task<IActionResult> Edit(string? userId)
        {
            // check parameter
            if (userId == null)
            {
                return NotFound();
            }

            if(User.IsInRole("Student") || User.IsInRole("Recruiter"))
            {
                // get currentUserId for security
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if(currentUserId != userId) { return Unauthorized();  }
            }

            // get user
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) { return NotFound(); }


            // populate major, state, type, company list
            var majorList = await _context.Majors.ToListAsync();
            ViewBag.Majors = new MultiSelectList(majorList, "MajorId", "MajorName");
            var stateList = new SelectList(Enum.GetValues(typeof(States)).Cast<States>());
            ViewBag.States = stateList;
            var positionTypeList = new SelectList(Enum.GetValues(typeof(PositionType)).Cast<PositionType>());
            ViewBag.PositionTypes = positionTypeList;
            // allow CSO to edit status and company
            if(User.IsInRole("CSO"))
            {
                var statusList = new SelectList(Enum.GetValues(typeof(Status)).Cast<Status>());
                ViewBag.Status = statusList;
                var companyList = await _context.Companies.ToListAsync();
                ViewBag.Company = new SelectList(companyList, "CompanyId", "CompanyName", user.Company);
            }
            // get user-to-edit's role
            var roles = await _userManager.GetRolesAsync(user);
            ViewBag.UserRole = roles.FirstOrDefault();

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string? userId, AppUser user, int? MajorId, int? CompanyId)
        {
            //check parameters
            if (user == null) { return NotFound();  }

            // get current user id
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // check that user is authorized to edit profile
            if (User.IsInRole("Student") || User.IsInRole("Recruiter"))
            {
                if (currentUserId != userId) { return Unauthorized(); }
            }

            // get user-to-edit's role
            var roles = await _userManager.GetRolesAsync(user);

            // check and handle update
            if (userId == user.Id)
            {
                // get current user data
                var userToUpdate = await _context.Users
                    .Include(u => u.Major)
                    .Include(u => u.Company)
                    .FirstOrDefaultAsync(u => u.Id == user.Id);
                if (userToUpdate == null) { return NotFound(); }

                // curr user role
                string currRole = roles.FirstOrDefault();

                // update default fields
                userToUpdate.FirstName = user.FirstName;
                userToUpdate.LastName = user.LastName;
                userToUpdate.MiddleInitial = user.MiddleInitial;
                userToUpdate.Email = user.Email;
                userToUpdate.UserName = user.Email;
                userToUpdate.NormalizedEmail = _userManager.NormalizeEmail(user.Email);
                userToUpdate.NormalizedUserName = _userManager.NormalizeName(user.Email);

                userToUpdate.Status = user.Status;

                if (currRole == "Student")
                {
                    userToUpdate.Birthday = user.Birthday;
                    userToUpdate.GPA = user.GPA;
                    userToUpdate.GraduationYear = user.GraduationYear;
                    userToUpdate.SSN = user.SSN;
                    userToUpdate.Street = user.Street;
                    userToUpdate.City = user.City;
                    userToUpdate.Zip = user.Zip;
                    userToUpdate.State = user.State;
                    userToUpdate.PositionType = user.PositionType;
                    if (MajorId != null) { userToUpdate.Major = await _context.Majors.FirstOrDefaultAsync(m => m.MajorId == MajorId); }
                }

                if(currRole == "Recruiter")
                {
                    if (CompanyId != null) { userToUpdate.Company = await _context.Companies.FirstOrDefaultAsync(c => c.CompanyId == CompanyId); }
                }

                // save changes
                try
                {
                    await _context.SaveChangesAsync();

                    if(currentUserId != user.Id)
                    {
                        return RedirectToAction("Index", "RoleAdmin");
                    }

                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (userToUpdate == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            // populate major, state, type, company list
            var majorList = await _context.Majors.ToListAsync();
            ViewBag.Majors = new MultiSelectList(majorList, "MajorId", "MajorName");
            var stateList = new SelectList(Enum.GetValues(typeof(States)).Cast<States>());
            ViewBag.States = stateList;
            var positionTypeList = new SelectList(Enum.GetValues(typeof(PositionType)).Cast<PositionType>());
            ViewBag.PositionTypes = positionTypeList;
            // allow CSO to edit status and company
            if (User.IsInRole("CSO"))
            {
                var statusList = new SelectList(Enum.GetValues(typeof(Status)).Cast<Status>());
                ViewBag.Status = statusList;
                var companyList = await _context.Companies.ToListAsync();
                ViewBag.Company = new SelectList(companyList, "CompanyId", "CompanyName", user.Company);
            }

            // update user role in ViewBag
            ViewBag.UserRole = roles.FirstOrDefault();


            // add back navigational properties
            user.Major = await _context.Majors.FirstOrDefaultAsync(m => m.User.Any(u => u.Id == user.Id));
            user.Company = await _context.Companies.FirstOrDefaultAsync(c => c.Users.Any(u => u.Id == user.Id));

            return View(user);
        }

        //Logic for change password
        // GET: /Account/ChangePassword
        [HttpGet]
        public ActionResult ChangePassword(string? userId)
        {
            // check parameter
            if (userId == null) { return NotFound(); }

            ViewBag.UserId = userId;

            return View();
        }

        // POST: /Account/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(string? userId, ChangePasswordViewModel cpvm)
        {
            // check parameter
            if(userId == null) { return NotFound();  }

            //if user forgot a field, send them back to 
            //change password page to try again
            if (ModelState.IsValid == false)
            {
                ViewBag.UserId = userId;
                return View(cpvm);
            }

            // get current user
            var currUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            //Find the logged in user using the UserManager
            var userLoggedIn = await _userManager.FindByIdAsync(userId);

            // check that user exists
            if(userLoggedIn == null ) { return Unauthorized();  }

            //if user is not the current user, make sure user is cso
            if(userId != currUserId && !User.IsInRole("CSO"))
            {
                return Unauthorized();
            }
            var result = await _userManager.ChangePasswordAsync(userLoggedIn, cpvm.OldPassword, cpvm.NewPassword);

            //if the attempt to change the password worked
            if (result.Succeeded)
            {

                if (userId == currUserId)
                {
                    //sign in the user with the new password
                    await _signInManager.SignInAsync(userLoggedIn, isPersistent: false);

                    //send the user back to the home page
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("UserDetails", new { id = userId });
                }

            }
            else //attempt to change the password didn't work
            {
                //Add all the errors from the result to the model state
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                //send the user back to the change password page to try again
                return View(cpvm);
            }
        }

        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LogOff()
        {
            //sign the user out of the application
            _signInManager.SignOutAsync();

            //send the user back to the home page
            return RedirectToAction("Index", "Home");
        }

        // Get user details
        public async Task<IActionResult> UserDetails(string? id)
        {
            // check that id was passed
            if (id == null)
            {
                return NotFound();
            }
            // query for user
            var user = await _context.Users.Include(u => u.Company).Include(u => u.Major)
                .FirstOrDefaultAsync(m => m.Id == id);
            // check that user exists
            if (user == null)
            {
                return NotFound();
            }
            // get user role
            var roles = await _userManager.GetRolesAsync(user);
            // create view model
            ProfileViewModel userView = new ProfileViewModel()
            {
                RoleType = roles.FirstOrDefault(),
                Id = user.Id,
                Status = user.Status,
                FirstName = user.FirstName,
                LastName = user.LastName,
                MiddleInitial = user.MiddleInitial,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                GPA = user.GPA,
                GraduationYear = user.GraduationYear,
                PositionType = user.PositionType,
                Major = user.Major?.MajorName,
                Company = user.Company?.Name,
            };

            if(User.IsInRole("Recruiter") || User.IsInRole("CSO"))
            {
                // get all open interview slots associated with recruiter's company
                var interviews = await _context.Interviews
                    .Include(i => i.Position)
                    .ThenInclude(i => i.Company)
                    .Where(i => i.Student == null)
                    .ToListAsync();

                // limit list to recruiter's company only
                if(User.IsInRole("Recruiter"))
                {
                    // get recruiter Id
                    var currUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (currUserId == null) { return NotFound(); }

                    var company = await _context.Companies.SingleOrDefaultAsync(c => c.Users.Any(u => u.Id == currUserId));
                    if (company == null) { return NotFound(); }

                    interviews = interviews.Where(i => i.Position.Company == company).ToList();
                }

                SelectList openInterviews = new SelectList(interviews, "InterviewId", "displayDetails");
                ViewBag.OpenInterviews = openInterviews;
            }

            return View("User", userView);
        }

        // Helper methods
        // Get all majors for the dropdown list
        private SelectList GetAllMajors()
        {
            // Get all majors from the database
            var majors = _context.Majors.ToList();
            // Create a SelectList from the majors
            return new SelectList(majors, "MajorId", "MajorName");
        }

        private SelectList GetAllCompanies()
        {
            // Get all majors from the database
            var companies = _context.Companies.ToList();
            // Create a SelectList from the majors
            return new SelectList(companies, "CompanyId", "Name");
        }
    }
}