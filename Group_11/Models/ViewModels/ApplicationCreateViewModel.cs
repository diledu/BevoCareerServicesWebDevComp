using System;
using Group_11.Models;

namespace Group_11.ViewModels
{
    public class ApplicationCreateViewModel
    {
        public int PositionId { get; set; }

        public string? Title { get; set; }
        public string? CompanyName { get; set; }
        public DateTime Deadline { get; set; }
        public string Status { get; set; } = "Pending";
    }
}
