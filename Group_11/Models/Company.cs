using System.ComponentModel.DataAnnotations;

namespace Group_11.Models
{
    public class Company
    {
        [Required(ErrorMessage = "CompanyId is required")]
        public int CompanyId { get; set; }
        [Required(ErrorMessage = "Company Name is required")]
        public string Name { get; set; }
        public string? Email { get; set; }
        public string? Description { get; set; }

        // Navigational Properties
        public List<Industry>? Industries { get; set; } = new List<Industry>();
        // Associated Recruiters
        public List<AppUser>? Users { get; set; } = new List<AppUser>();
        // Positions posted by the company
        public List<Position>? Positions { get; set; } = new List<Position>();

    }
}
