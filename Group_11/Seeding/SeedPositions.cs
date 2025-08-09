using Group_11.DAL;
using Group_11.Models;
using Microsoft.EntityFrameworkCore;

namespace Group_11.Seeding
{
    public class SeedPositions
    {
        public static void SeedPositionData(AppDbContext context)
        {
            if (!context.Positions.Any())
            {
                // get majors
                var accounting = context.Majors.FirstOrDefault(m => m.MajorName == "Accounting");
                var finance = context.Majors.FirstOrDefault(m => m.MajorName == "Finance");
                var marketing = context.Majors.FirstOrDefault(m => m.MajorName == "Marketing");
                var mis = context.Majors.FirstOrDefault(m => m.MajorName == "MIS");
                var scm = context.Majors.FirstOrDefault(m => m.MajorName == "Supply Chain Management");
                var honors = context.Majors.FirstOrDefault(m => m.MajorName == "Business Honors");
                var management = context.Majors.FirstOrDefault(m => m.MajorName == "Management");

                // get companies
                var academy = context.Companies.Include(c => c.Users).FirstOrDefault(c => c.Name == "Academy Sports & Outdoors");
                var accenture = context.Companies.Include(c => c.Users).FirstOrDefault(c => c.Name == "Accenture");
                var adlucent = context.Companies.Include(c => c.Users).FirstOrDefault(c => c.Name == "Adlucent");
                var capitalOne = context.Companies.Include(c => c.Users).FirstOrDefault(c => c.Name == "Capital One");
                var deloitte = context.Companies.Include(c => c.Users).FirstOrDefault(c => c.Name == "Deloitte");
                var hilton = context.Companies.Include(c => c.Users).FirstOrDefault(c => c.Name == "Hilton Worldwide");
                var shell = context.Companies.Include(c => c.Users).FirstOrDefault(c => c.Name == "Shell");
                var ti = context.Companies.Include(c => c.Users).FirstOrDefault(c => c.Name == "Texas Instruments");

                if (academy == null || accenture == null || adlucent == null ||
                    capitalOne == null || deloitte == null || hilton == null ||
                    shell == null || ti == null) {
                    throw new Exception("One or more required Companies were not found in the database.");
                }

                if (accounting == null || finance == null || marketing == null ||
                    mis == null || scm == null || honors == null || management == null)
                {
                    throw new Exception("One or more required Majors were not found in the database.");
                }


                AppUser GetRecruiter(Company company) => company.Users.FirstOrDefault();

                List<Position> positions = new List<Position>
                {
                    new Position
                    {
                        Title = "Financial Planning Intern",
                        Description = "",
                        Type = PositionType.Internship,
                        Company = academy,
                        Recruiter = GetRecruiter(academy),
                        City = "Orlando",
                        State = States.FL,
                        Deadline = new DateTime(2025, 6, 1),
                        Majors = new List<Major> { finance, accounting, honors }
                    },
                    new Position
                    {
                        Title = "Digital Product Manager",
                        Description = "",
                        Type = PositionType.Fulltime,
                        Company = academy,
                        Recruiter = GetRecruiter(academy),
                        City = "Houston",
                        State = States.TX,
                        Deadline = new DateTime(2025, 6, 1),
                        Majors = new List<Major> { mis, marketing, honors, management }
                    },
                    new Position
                    {
                        Title = "Consultant",
                        Description = "Full-time consultant position",
                        Type = PositionType.Fulltime,
                        Company = accenture,
                        Recruiter = GetRecruiter(accenture),
                        City = "Houston",
                        State = States.TX,
                        Deadline = new DateTime(2025, 4, 15),
                        Majors = new List<Major> { mis, accounting, honors }
                    },
                    new Position
                    {
                        Title = "Digital Intern",
                        Description = "",
                        Type = PositionType.Internship,
                        Company = accenture,
                        Recruiter = GetRecruiter(accenture),
                        City = "Dallas",
                        State = States.TX,
                        Deadline = new DateTime(2025, 5, 20),
                        Majors = new List<Major> { mis, marketing }
                    },
                    new Position
                    {
                        Title = "Marketing Intern",
                        Description = "Help our marketing team develop new advertising strategies for local Austin businesses",
                        Type = PositionType.Internship,
                        Company = adlucent,
                        Recruiter = GetRecruiter(adlucent),
                        City = "Austin",
                        State = States.TX,
                        Deadline = new DateTime(2025, 5, 2),
                        Majors = new List<Major> { marketing }
                    },
                    new Position
                    {
                        Title = "Web Development",
                        Description = "Developing a great new website for customer portfolio management",
                        Type = PositionType.Fulltime,
                        Company = capitalOne,
                        Recruiter = GetRecruiter(capitalOne),
                        City = "Richmond",
                        State = States.VA,
                        Deadline = new DateTime(2025, 3, 14),
                        Majors = new List<Major> { mis }
                    },
                    new Position
                    {
                        Title = "Analyst Development Program",
                        Description = "",
                        Type = PositionType.Internship,
                        Company = capitalOne,
                        Recruiter = GetRecruiter(capitalOne),
                        City = "Plano",
                        State = States.TX,
                        Deadline = new DateTime(2025, 5, 20),
                        Majors = new List<Major> { finance, mis, honors }
                    },
                    new Position
                    {
                        Title = "Accounting Intern",
                        Description = "Work in our audit group",
                        Type = PositionType.Internship,
                        Company = deloitte,
                        Recruiter = GetRecruiter(deloitte),
                        City = "Austin",
                        State = States.TX,
                        Deadline = new DateTime(2025, 5, 3),
                        Majors = new List<Major> { accounting }
                    },
                    new Position
                    {
                        Title = "Account Manager",
                        Description = "",
                        Type = PositionType.Fulltime,
                        Company = deloitte,
                        Recruiter = GetRecruiter(deloitte),
                        City = "Dallas",
                        State = States.TX,
                        Deadline = new DateTime(2025, 2, 25),
                        Majors = new List<Major> { accounting, honors }
                    },
                    new Position
                    {
                        Title = "Amenities Analytics Intern",
                        Description = "Help analyze our amenities and customer rewards programs",
                        Type = PositionType.Internship,
                        Company = hilton,
                        Recruiter = GetRecruiter(hilton),
                        City = "New York",
                        State = States.NY,
                        Deadline = new DateTime(2025, 3, 30),
                        Majors = new List<Major> { finance, mis, marketing, honors }
                    },
                    new Position
                    {
                        Title = "Supply Chain Internship",
                        Description = "",
                        Type = PositionType.Internship,
                        Company = shell,
                        Recruiter = GetRecruiter(shell),
                        City = "Houston",
                        State = States.TX,
                        Deadline = new DateTime(2025, 5, 5),
                        Majors = new List<Major> { scm }
                    },
                    new Position
                    {
                        Title = "Procurements Associate",
                        Description = "Handle procurement and vendor accounts",
                        Type = PositionType.Fulltime,
                        Company = shell,
                        Recruiter = GetRecruiter(shell),
                        City = "Houston",
                        State = States.TX,
                        Deadline = new DateTime(2025, 5, 30),
                        Majors = new List<Major> { scm }
                    },
                    new Position
                    {
                        Title = "Sales Rotational Program",
                        Description = "",
                        Type = PositionType.Fulltime,
                        Company = ti,
                        Recruiter = GetRecruiter(ti),
                        City = "Dallas",
                        State = States.TX,
                        Deadline = new DateTime(2025, 5, 30),
                        Majors = new List<Major> { marketing, management, finance, accounting }
                    }
                };

                context.Positions.AddRange(positions);
                context.SaveChanges();
            }
        }
    }
}
