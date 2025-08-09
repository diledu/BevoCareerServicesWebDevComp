using System.ComponentModel.DataAnnotations;

namespace Group_11.Models
{
    public class Position
    {
        [Required(ErrorMessage = "PositionId is required")]
        public int PositionId { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string Title { get; set; }
        public string? Description { get; set; }
        [Required(ErrorMessage = "Deadline is required")]
        public DateTime Deadline { get; set; }
        [Required(ErrorMessage = "Type is required")]
        public PositionType Type { get; set; }
        public string? City { get; set; }
        public States? State { get; set; }


        //Navigational Properties
        public AppUser? Recruiter { get; set; }
        public Company? Company { get; set; }
        public List<Major> Majors { get; set; } = new List<Major>();
        public List<Application>? Applications { get; set; } = new List<Application>();

    }
}
