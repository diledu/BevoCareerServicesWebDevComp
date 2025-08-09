using System.ComponentModel.DataAnnotations;

namespace Group_11.Models
{
    public class Major
    {
        [Required(ErrorMessage = "MajorId is required")]
        public int MajorId { get; set; }
        [Required(ErrorMessage = "MajorName is required")]
        public string MajorName { get; set; }   

        // Navigational Properties
        // Associated Students
        public List<AppUser>? User { get; set; }
        // Associated Positions
        public List<Position>? Positions { get; set; }
    }
}
