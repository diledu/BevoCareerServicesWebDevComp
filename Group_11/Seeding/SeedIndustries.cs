using Group_11.DAL;
using Group_11.Models;

namespace Group_11.Seeding
{
    public class SeedIndustries
    {
        public static void SeedIndustryData(AppDbContext context)
        {
            if (!context.Industries.Any())
            {
                List<Industry> allIndustries = new List<Industry>
            {
                new Industry { IndustryName = "Accounting" },
                new Industry { IndustryName = "Consulting" },
                new Industry { IndustryName = "Energy" },
                new Industry { IndustryName = "Engineering" },
                new Industry { IndustryName = "Chemicals" },
                new Industry { IndustryName = "Technology" },
                new Industry { IndustryName = "Financial Services" },
                new Industry { IndustryName = "Manufacturing" },
                new Industry { IndustryName = "Hospitality" },
                new Industry { IndustryName = "Marketing" },
                new Industry { IndustryName = "Retail" },
                new Industry { IndustryName = "Insurance" },
                new Industry { IndustryName = "Real-Estate" },
                new Industry { IndustryName = "Transportation" }
            };

                context.Industries.AddRange(allIndustries);
                context.SaveChanges();
            }
        }
    }
}
