using System.ComponentModel.DataAnnotations;

namespace Group_11.Models
{
    public class Industry
    {
        public int IndustryId { get; set; }
        [Required]
        public string IndustryName { get; set; }

        // Navigation
        public List<Company> Companies { get; set; }
    }
}
