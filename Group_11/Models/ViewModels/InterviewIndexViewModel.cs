namespace Group_11.Models.ViewModels
{
    public class InterviewIndexViewModel
    {
        public IEnumerable<Interview> AllInterviews { get; set; }
        public IEnumerable<Interview> UserInterviews { get; set; }
    }
}
