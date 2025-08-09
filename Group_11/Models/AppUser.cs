using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Group_11.Models
{
    // ENUM for positions
    public enum PositionType
    {
        Fulltime, 
        Internship
    };
    // ENUM for user status
    public enum Status
    {
        Active,
        Inactive
    };
    // ENUM for location: states
    public enum States
    {
        AL, AK, AZ, AR, CA, CO, CT, DE, FL, GA, HI, ID, IL, IN, IA, KS, KY, LA, ME, MD, MA, MI, MN, MS, MO, MT, NE, NV, NH, NJ, NM, NY, NC, ND, OH, OK, OR, PA, RI, SC, SD, TN, TX, UT, VT, VA, WA, WV, WI, WY
    };

    public class AppUser : IdentityUser {

        [Required(ErrorMessage = "First Name is required")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        public string? MiddleInitial { get; set; }
        [Required(ErrorMessage = "Last Name is required")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public DateTime? Birthday { get; set; }
        public string? SSN { get; set; }
        public string? Street {  get; set; }
        public string? City { get; set; }
        public string? Zip { get; set; }
        public States? State { get; set; }

        [Range(0, 4.00)]
        public decimal? GPA { get; set; }
        [Range(2000, 2100, ErrorMessage = "Graduation year must be between 2000 and 2100")]
        public int? GraduationYear { get; set; }
        public PositionType? PositionType { get; set; }
        [Required(ErrorMessage = "Status is required")]
        public Status? Status { get; set; }

        // Navigational Properties for relationships
        // Users may not have associated properties depending on Role
        public Major? Major { get; set; }
        public Company? Company { get; set; }
        // siloed list for student interviews >> help model roles
        public List<Interview>? InterviewsAsStudent { get; set; } = new List<Interview>();
        // siloed list for recruiter interviews >> help model roles
        public List<Interview>? InterviewsAsInterviewer{ get; set; } = new List<Interview>();
        public List<Application>? Applications { get; set; } = new List<Application>();
        public List<Position>? Positions { get; set; } = new List<Position>();



        // read properties
        public string FullName => FirstName + " " + LastName;
    }
}
