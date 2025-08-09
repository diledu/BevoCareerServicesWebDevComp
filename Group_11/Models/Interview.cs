using System.ComponentModel.DataAnnotations;

namespace Group_11.Models
{
    public class Interview
    {
        [Required(ErrorMessage = "InterviewId is required")]
        public int InterviewId { get; set; }
        [Range(1, 4)]
        [Required(ErrorMessage = "Room number is required")]
        public int Room {  get; set; }
        [Required(ErrorMessage = "Interview Date is required")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime DateTime { get; set; }

        // Navigational Properties
        // Student may not be asssociated at the time of creation
        public AppUser? Student { get; set; }
        public AppUser? Interviewer { get; set; }
        public Position? Position { get; set; }

        // read properties
        public string displayDetails => $"{Position?.Title ?? "Unknown Position"}: {DateTime:dddd, MMM d 'at' h:mm tt} — Room {Room}";
    }
}
