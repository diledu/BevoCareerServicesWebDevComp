using System.ComponentModel.DataAnnotations;

namespace Group_11.Models
{
    public class Application
    {
        public enum AppStatus
        {
            Pending,
            Accepted,
            Rejected,
            Withdrawn
        };

        [Required(ErrorMessage = "ApplicationId is required")]
        public int ApplicationId { get; set; }
        [Required(ErrorMessage = "Status is required")]
        public AppStatus Status { get; set; }
        public DateTime DateTime { get; set; } = DateTime.Now;

        // Navigational Properties
        public AppUser Student { get; set; }
        public Position Position { get; set; }
    }
}
