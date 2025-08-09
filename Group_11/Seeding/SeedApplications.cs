using Group_11.DAL;
using Group_11.Models;

namespace Group_11.Seeding
{
    public class SeedApplications
    {
        public static void SeedApplicationData(AppDbContext context)
        {
            if (!context.Applications.Any())
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
                var jim = context.Users.FirstOrDefault(u => u.FirstName == "Jim Bob" && u.LastName == "Evans");
                var reagan = context.Users.FirstOrDefault(u => u.FirstName == "Reagan" && u.LastName == "Wood");

                // get positions
                var accountManager = context.Positions.FirstOrDefault(p => p.Title == "Account Manager" && p.Company.Name == "Deloitte");
                var webDev = context.Positions.FirstOrDefault(p => p.Title == "Web Development" && p.Company.Name == "Capital One");
                var amenitiesIntern = context.Positions.FirstOrDefault(p => p.Title == "Amenities Analytics Intern" && p.Company.Name == "Hilton Worldwide");
                var supplyChainIntern = context.Positions.FirstOrDefault(p => p.Title == "Supply Chain Internship" && p.Company.Name == "Shell");
                var accountingIntern = context.Positions.FirstOrDefault(p => p.Title == "Accounting Intern" && p.Company.Name == "Deloitte");

                List<Application> applications = new List<Application>
                {
                    new Application { Student = eric, Position = accountManager, Status = Application.AppStatus.Accepted },
                    new Application { Student = christopher, Position = webDev, Status = Application.AppStatus.Accepted },
                    new Application { Student = eryn, Position = amenitiesIntern, Status = Application.AppStatus.Accepted },
                    new Application { Student = tesa, Position = amenitiesIntern, Status = Application.AppStatus.Accepted },
                    new Application { Student = lim, Position = amenitiesIntern, Status = Application.AppStatus.Accepted },
                    new Application { Student = brad, Position = supplyChainIntern, Status = Application.AppStatus.Accepted },
                    new Application { Student = sarah, Position = supplyChainIntern, Status = Application.AppStatus.Accepted },
                    new Application { Student = chuck, Position = accountingIntern, Status = Application.AppStatus.Accepted },
                    new Application { Student = jim, Position = accountManager, Status = Application.AppStatus.Accepted },
                    new Application { Student = reagan, Position = accountManager, Status = Application.AppStatus.Pending }
                };

                context.Applications.AddRange(applications);
                context.SaveChanges();
            }
        }
    }
}
