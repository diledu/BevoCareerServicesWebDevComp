using Group_11.DAL;
using Group_11.Models;

namespace Group_11.Seeding
{
    public class SeedMajors
    {
        public static void SeedMajorData(AppDbContext context)
        {
            if (!context.Majors.Any())
            {
                List<Major> majors = new List<Major>()
                {
                    new Major() { MajorName = "MIS" },
                    new Major() { MajorName = "Finance" },
                    new Major() { MajorName = "Accounting" },
                    new Major() { MajorName = "Supply Chain Management" },
                    new Major() { MajorName = "Marketing" },
                    new Major() { MajorName = "Business Honors" }
                };

                context.Majors.AddRange(majors);
                context.SaveChanges();
            }
        }

    }
}
