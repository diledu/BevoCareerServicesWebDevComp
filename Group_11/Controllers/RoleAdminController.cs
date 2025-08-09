using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

using Group_11.DAL;
using Group_11.Models;

using Group_11.Models.ViewModels;
using System;

namespace Group_11.Controllers
{
    //TODO: Uncomment this line once you have roles working correctly
    [Authorize(Roles = "CSO")]
    public class RoleAdminController : Controller
    {
        //create private variables for the services needed in this controller
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        //RoleAdminController constructor
        public RoleAdminController(AppDbContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //populate the values of the variables passed into the controller
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: /RoleAdmin/
        public async Task<ActionResult> Index()
        {
            var allUsers = await _context.Users
                .Include(u => u.Major)
                .Include(u => u.Company)
                .ToListAsync();

            // populate user lists
            var studentList = allUsers.Where(u => u.Major != null);
            ViewBag.Students = studentList;
            var recruiterList = allUsers.Where(u => u.Company != null);
            ViewBag.Recruiters = recruiterList;
            var csoList = allUsers.Where(u => u.Major == null && u.Company == null);
            ViewBag.CSO = csoList;

            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create([Required] string name)
        {
            if (ModelState.IsValid)
            {
                //attempt to create the new role using the role manager
                IdentityResult result = await _roleManager.CreateAsync(new IdentityRole(name));

                //if the role was created successfully, take the user to the index page
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else //role was not added succesfully, so add errors to model 
                //and let the user try again
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            //if code gets this far, we need to show an error
            return View(name);
        }
        public async Task<ActionResult> Edit(string id)
        {
            //look up the role requested by the user
            IdentityRole role = await _roleManager.FindByIdAsync(id);

            //create a list for the members of the role
            List<AppUser> RoleMembers = new List<AppUser>();

            //create a list for the non-members of the role
            List<AppUser> RoleNonMembers = new List<AppUser>();

            //through ALL the users and decide if they are in the role(member) or not (non-member)
            foreach (AppUser user in _userManager.Users)
            {
                if (await _userManager.IsInRoleAsync(user, role.Name) == true) //user is in the role
                {
                    //add the user to the list of members
                    RoleMembers.Add(user);
                }
                else //user is NOT in the role
                {
                    RoleNonMembers.Add(user);
                }
            }

            //create a new instance of the role edit model
            RoleEditModel rem = new RoleEditModel();

            //populate the properties of the role edit model
            rem.Role = role; //role looked up from database
            rem.RoleMembers = RoleMembers; //list of users in the role
            rem.RoleNonMembers = RoleNonMembers; //list of users NOT in the role

            //send user to view with populated role edit model
            return View(rem);
        }

        //NOTE: nullable + Edit.cshtml is wrong --> change Model.RoleMembers.Count == 0 for Remove
        [HttpPost]
        public async Task<ActionResult> Edit(RoleModificationModel rmm)
        {
            //create a result to refer to later
            IdentityResult result;

            //if RoleModificationModel is valid, add new users
            if (ModelState.IsValid)
            {
                //if there are users to add, then add them
                if (rmm.IdsToAdd != null)
                {
                    foreach (string userId in rmm.IdsToAdd)
                    {
                        //find the user in the database using their id
                        AppUser user = await _userManager.FindByIdAsync(userId);

                        //attempt to add the user to the role using the UserManager
                        result = await _userManager.AddToRoleAsync(user, rmm.RoleName);

                        //TODO: check this
                        //if attempt to add user to role didn't work, show user the error page
                        if (result.Succeeded == false)
                        {
                            //send user to error page
                            return View("Error", result.Errors);
                        }
                    }
                }

                //if there are users to remove from the role, remove them
                if (rmm.IdsToDelete != null)
                {
                    //loop through all the ids to remove from role
                    foreach (string userId in rmm.IdsToDelete)
                    {
                        //find the user in the database using their id
                        AppUser user = await _userManager.FindByIdAsync(userId);

                        //attempt to remove the user from the role using the UserManager
                        result = await _userManager.RemoveFromRoleAsync(user, rmm.RoleName);

                        //if attempt to remove the user from role didn't work, show the error page
                        if (result.Succeeded == false)
                        {
                            //show user the error page
                            return View("Error", result.Errors);
                        }
                    }
                }

                //this is the happy path - all edits worked
                //take the user back to the RoleAdmin Index page
                return RedirectToAction("Index");
            }

            //this is a sad path - the role was not found
            //show the user the error page
            return View("Error", new string[] { "Role Not Found" });
        }
    }
}