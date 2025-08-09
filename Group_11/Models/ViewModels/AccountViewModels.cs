using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using Group_11.Models;

namespace Group_11.Models.ViewModels
{
    //NOTE: This is the view model used to allow the user to login
    //The user only needs the email and password to login
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    //NOTE: This is the view model used to register a user
    //When the user registers, they only need to specify the
    //properties listed in this model
    public class RegisterViewModel
    {
        //NOTE: Here is the property for email
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        //NOTE: Here is the property for phone number
        [Required(ErrorMessage = "Phone number is required")]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }


        //First name is provided as an example
        [Required(ErrorMessage = "First name is required.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last name is required.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }


        //NOTE: Here is the logic for putting in a password
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        // optional parameter to specify roleType >> for CSO signup usage
        public string? RoleType { get; set; } 

        // NOTE: Added optional fields for Student data
        public string? MiddleInitial { get; set; }
        public DateTime? Birthday { get; set; }
        public string? SSN { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? Zip { get; set; }
        public States? State { get; set; }
        public int? Major { get; set; }
        [Range(0, 4.00)]
        public decimal? GPA { get; set; }
        [Range(2000, 2100, ErrorMessage = "Graduation year must be between 2000 and 2100")]
        public int? GraduationYear { get; set; }
        public PositionType? PositionType { get; set; }

        // NOTE: Added optional fields for Recruiter data
        public int? Company { get; set; }
    }

    //NOTE: This is the view model used to allow the user to 
    //change their password
    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    //NOTE: This is the view model used to display basic user information
    //on the index page
    public class IndexViewModel
    {
        public bool HasPassword { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string UserID { get; set; }
    }

    // View Model for Profile View
    public class ProfileViewModel
    {
        // optional parameter to specify roleType >> for CSO signup usage
        public string? RoleType { get; set; }
        public string? Id { get; set; }
        public Status? Status { get; set; }

        // User data
        [EmailAddress]
        [Display(Name = "Email")]
        public string? Email { get; set; }
        [Phone]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }


        // NOTE: Added optional fields for Student data
        public string? MiddleInitial { get; set; }
            // store major string for display
        public string? Major { get; set; }
        [Range(0, 4.00)]
        public decimal? GPA { get; set; }
        [Range(2000, 2100, ErrorMessage = "Graduation year must be between 2000 and 2100")]
        public int? GraduationYear { get; set; }
        public PositionType? PositionType { get; set; }

        // NOTE: Added optional fields for Recruiter data
            // store company string for display
        public string? Company { get; set; }
    }
}
