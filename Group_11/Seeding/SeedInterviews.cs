using Group_11.DAL;
using Group_11.Models;

namespace Group_11.Seeding
{
    public class SeedInterviews
    {
        public static void SeedInterviewData(AppDbContext context)
        {
            if (!context.Interviews.Any())
            {
                // get students
                var eric = context.Users.FirstOrDefault(u => u.FirstName == "Eric" && u.LastName == "Stuart");
                var christopher = context.Users.FirstOrDefault(u => u.FirstName == "Christopher" && u.LastName == "Baker");
                var eryn = context.Users.FirstOrDefault(u => u.FirstName == "Eryn" && u.LastName == "Rice");
                var tesa = context.Users.FirstOrDefault(u => u.FirstName == "Tesa" && u.LastName == "Freeley");
                var lim = context.Users.FirstOrDefault(u => u.FirstName == "Lim" && u.LastName == "Chou");
                var brad = context.Users.FirstOrDefault(u => u.FirstName == "Brad" && u.LastName == "Ingram");
                var sarah = context.Users.FirstOrDefault(u => u.FirstName == "Sarah" && u.LastName == "Saunders");
                var chuck = context.Users.FirstOrDefault(u => u.FirstName == "Chuck" && u.LastName == "Luce");

                // get interviewers
                var clarence = context.Users.FirstOrDefault(u => u.FirstName == "Clarence" && u.LastName == "Martin");
                var gregory = context.Users.FirstOrDefault(u => u.FirstName == "Gregory" && u.LastName == "Martinez");
                var rachel = context.Users.FirstOrDefault(u => u.FirstName == "Rachel" && u.LastName == "Taylor");
                var ernest = context.Users.FirstOrDefault(u => u.FirstName == "Ernest" && u.LastName == "Lowe");
                var kelly = context.Users.FirstOrDefault(u => u.FirstName == "Kelly" && u.LastName == "Nelson");

                // get positions
                var accountManager = context.Positions.FirstOrDefault(p => p.Title == "Account Manager");
                var webDev = context.Positions.FirstOrDefault(p => p.Title == "Web Development");
                var amenitiesIntern = context.Positions.FirstOrDefault(p => p.Title == "Amenities Analytics Intern");
                var supplyChainIntern = context.Positions.FirstOrDefault(p => p.Title == "Supply Chain Internship");
                var accountingIntern = context.Positions.FirstOrDefault(p => p.Title == "Accounting Intern");

                // create interview seed data
                List<Interview> interviews = new List<Interview>
                {
                    new Interview
                    {
                        Student = eric,
                        Interviewer = clarence,
                        Position = accountManager,
                        Room = 1,
                        DateTime = new DateTime(2025, 5, 13, 10, 0, 0)
                    },
                    new Interview
                    {
                        Student = christopher,
                        Interviewer = gregory,
                        Position = webDev,
                        Room = 2,
                        DateTime = new DateTime(2025, 5, 16, 14, 0, 0)
                    },
                    new Interview
                    {
                        Student = eryn,
                        Interviewer = rachel,
                        Position = amenitiesIntern,
                        Room = 1,
                        DateTime = new DateTime(2025, 4, 1, 9, 0, 0)
                    },
                    new Interview
                    {
                        Student = tesa,
                        Interviewer = rachel,
                        Position = amenitiesIntern,
                        Room = 1,
                        DateTime = new DateTime(2025, 4, 1, 10, 0, 0)
                    },
                    new Interview
                    {
                        Student = lim,
                        Interviewer = rachel,
                        Position = amenitiesIntern,
                        Room = 4,
                        DateTime = new DateTime(2025, 4, 2, 15, 0, 0)
                    },
                    new Interview
                    {
                        Student = brad,
                        Interviewer = ernest,
                        Position = supplyChainIntern,
                        Room = 1,
                        DateTime = new DateTime(2025, 5, 10, 9, 0, 0)
                    },
                    new Interview
                    {
                        Student = sarah,
                        Interviewer = ernest,
                        Position = supplyChainIntern,
                        Room = 1,
                        DateTime = new DateTime(2025, 5, 10, 11, 0, 0)
                    },
                    new Interview
                    {
                        Student = chuck,
                        Interviewer = kelly,
                        Position = accountingIntern,
                        Room = 4,
                        DateTime = new DateTime(2025, 5, 16, 11, 0, 0)
                    }
                };

                context.Interviews.AddRange(interviews);
                context.SaveChanges();
            }
        }
    }
}
