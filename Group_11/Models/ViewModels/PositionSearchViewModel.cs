using Group_11.Models;
using System.Collections.Generic;

namespace Group_11.Models
{
    public class PositionSearchViewModel
    {
        public string? CompanyName { get; set; }
        public string? IndustryName { get; set; }
        public string? PositionType { get; set; }
        public string? MajorName { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }

        public List<Position> Results { get; set; } = new List<Position>();
    }
}
