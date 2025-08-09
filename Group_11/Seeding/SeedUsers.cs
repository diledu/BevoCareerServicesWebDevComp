using Microsoft.AspNetCore.Identity;

using Group_11.Utilities;
using Group_11.DAL;
using Group_11.Models;
using Microsoft.Identity.Client;

namespace Group_11.Seeding
{
    public static class SeedUsers
    {
        public async static Task<IdentityResult> SeedAllUsers(UserManager<AppUser> userManager, AppDbContext context)
        {
            // instantiate majors
            var mis = context.Majors.FirstOrDefault(m => m.MajorName == "MIS");
            var finance = context.Majors.FirstOrDefault(m => m.MajorName == "Finance");
            var accounting = context.Majors.FirstOrDefault(m => m.MajorName == "Accounting");
            var scm = context.Majors.FirstOrDefault(m => m.MajorName == "Supply Chain Management");
            var marketing = context.Majors.FirstOrDefault(m => m.MajorName == "Marketing");
            var honors = context.Majors.FirstOrDefault(m => m.MajorName == "Business Honors");

            // check that majors have been seeded & pulled
            if (mis == null || finance == null || accounting == null || scm == null || marketing == null || honors == null)
            {
                throw new Exception("One or more Majors could not be found. Please ensure majors have been seeded before seeding users.");
            }

            //Create a list of AddUserModels
            List<AddUserModel> AllUsers = new List<AddUserModel>();

            // Seed Students
            
            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "cbaker@example.com",
                    Email = "cbaker@example.com",
                    PhoneNumber = "152-275-7212",
                    FirstName = "Christopher",
                    MiddleInitial = "L",
                    LastName = "Baker",
                    Birthday = new DateTime(2001, 8, 2),
                    SSN = "425-46-3915",
                    Street = "1 David Park",
                    City = "Austin",
                    State = States.TX,
                    Zip = "78705",
                    GPA = 3.91m,
                    GraduationYear = 2023,
                    PositionType = PositionType.Fulltime,
                    Status = Status.Active,
                    Major = mis
                },
                Password = "bookworm",
                RoleName = "Student"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "limchou@gogle.com",
                    Email = "limchou@gogle.com",
                    PhoneNumber = "728-717-9608",
                    FirstName = "Lim",
                    LastName = "Chou",
                    Birthday = new DateTime(2003, 4, 6),
                    SSN = "498-04-2607",
                    Street = "703 Anthes Lane",
                    City = "Austin",
                    State = States.TX,
                    Zip = "78729",
                    GPA = 2.63m,
                    GraduationYear = 2024,
                    PositionType = PositionType.Internship,
                    Status = Status.Active,
                    Major = finance
                },
                Password = "allyrally",
                RoleName = "Student"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "j.b.evans@aheca.org",
                    Email = "j.b.evans@aheca.org",
                    PhoneNumber = "878-921-1122",
                    FirstName = "Jim Bob",
                    LastName = "Evans",
                    Birthday = new DateTime(2001, 10, 8),
                    SSN = "699-10-0990",
                    Street = "51 Miller Park",
                    City = "Austin",
                    State = States.TX,
                    Zip = "78705",
                    GPA = 2.64m,
                    GraduationYear = 2023,
                    PositionType = PositionType.Fulltime,
                    Status = Status.Active,
                    Major = accounting
                },
                Password = "billyboy",
                RoleName = "Student"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "tfreeley@minnetonka.ci.us",
                    Email = "tfreeley@minnetonka.ci.us",
                    PhoneNumber = "327-105-4962",
                    FirstName = "Tesa",
                    MiddleInitial = "P",
                    LastName = "Freeley",
                    Birthday = new DateTime(1996, 9, 12),
                    SSN = "518-30-9478",
                    Street = "97327 Express Avenue",
                    City = "College Station",
                    State = States.TX,
                    Zip = "77840",
                    GPA = 3.98m,
                    GraduationYear = 2023,
                    PositionType = PositionType.Internship,
                    Status = Status.Active,
                    Major = accounting
                },
                Password = "dustydusty",
                RoleName = "Student"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "ingram@jack.com",
                    Email = "ingram@jack.com",
                    PhoneNumber = "965-996-5936",
                    FirstName = "Brad",
                    MiddleInitial = "S",
                    LastName = "Ingram",
                    Birthday = new DateTime(2001, 2, 6),
                    SSN = "049-54-7605",
                    Street = "96 Stang Hill",
                    City = "New Braunfels",
                    State = States.TX,
                    Zip = "78132",
                    GPA = 3.72m,
                    GraduationYear = 2024,
                    PositionType = PositionType.Internship,
                    Status = Status.Active,
                    Major = scm
                },
                Password = "joejoejoe",
                RoleName = "Student"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "cluce@gogle.com",
                    Email = "cluce@gogle.com",
                    PhoneNumber = "782-613-4758",
                    FirstName = "Chuck",
                    MiddleInitial = "B",
                    LastName = "Luce",
                    Birthday = new DateTime(2001, 12, 23),
                    SSN = "008-92-9464",
                    Street = "5 Carberry Point",
                    City = "Navasota",
                    State = States.TX,
                    Zip = "77868",
                    GPA = 3.87m,
                    GraduationYear = 2024,
                    PositionType = PositionType.Internship,
                    Status = Status.Active,
                    Major = accounting
                },
                Password = "meganr34",
                RoleName = "Student"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "erynrice@aoll.com",
                    Email = "erynrice@aoll.com",
                    PhoneNumber = "589-264-1451",
                    FirstName = "Eryn",
                    MiddleInitial = "M",
                    LastName = "Rice",
                    Birthday = new DateTime(2004, 4, 29),
                    SSN = "320-66-2437",
                    Street = "37080 Darwin Parkway",
                    City = "South Padre Island",
                    State = States.TX,
                    Zip = "78597",
                    GPA = 3.92m,
                    GraduationYear = 2026,
                    PositionType = PositionType.Internship,
                    Status = Status.Active,
                    Major = marketing
                },
                Password = "radgirl",
                RoleName = "Student"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "saunders@pen.com",
                    Email = "saunders@pen.com",
                    PhoneNumber = "751-939-8193",
                    FirstName = "Sarah",
                    MiddleInitial = "J",
                    LastName = "Saunders",
                    Birthday = new DateTime(2002, 2, 5),
                    SSN = "390-15-8396",
                    Street = "77 International Drive",
                    City = "Austin",
                    State = States.TX,
                    Zip = "78720",
                    GPA = 3.16m,
                    GraduationYear = 2024,
                    PositionType = PositionType.Internship,
                    Status = Status.Active,
                    Major = scm
                },
                Password = "slowwind",
                RoleName = "Student"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "estuart@anchor.net",
                    Email = "estuart@anchor.net",
                    PhoneNumber = "762-772-8288",
                    FirstName = "Eric",
                    MiddleInitial = "D",
                    LastName = "Stuart",
                    Birthday = new DateTime(2003, 4, 17),
                    SSN = "316-21-6702",
                    Street = "20644 Badeau Point",
                    City = "Corpus Christi",
                    State = States.TX,
                    Zip = "78412",
                    GPA = 3.58m,
                    GraduationYear = 2023,
                    PositionType = PositionType.Fulltime,
                    Status = Status.Active,
                    Major = honors
                },
                Password = "stewball",
                RoleName = "Student"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "rwood@voyager.net",
                    Email = "rwood@voyager.net",
                    PhoneNumber = "958-745-9445",
                    FirstName = "Reagan",
                    MiddleInitial = "B",
                    LastName = "Wood",
                    Birthday = new DateTime(2001, 12, 25),
                    SSN = "512-444-3434",
                    Street = "95 Longview Point",
                    City = "Austin",
                    State = States.TX,
                    Zip = "78712",
                    GPA = 3.78m,
                    GraduationYear = 2023,
                    PositionType = PositionType.Fulltime,
                    Status = Status.Active,
                    Major = accounting
                },
                Password = "xcellent",
                RoleName = "Student"
            });

            // get companies
            var accenture = context.Companies.FirstOrDefault(c => c.Name == "Accenture");
            var shell = context.Companies.FirstOrDefault(c => c.Name == "Shell");
            var deloitte = context.Companies.FirstOrDefault(c => c.Name == "Deloitte");
            var capitalOne = context.Companies.FirstOrDefault(c => c.Name == "Capital One");
            var ti = context.Companies.FirstOrDefault(c => c.Name == "Texas Instruments");
            var hilton = context.Companies.FirstOrDefault(c => c.Name == "Hilton Worldwide");
            var adlucent = context.Companies.FirstOrDefault(c => c.Name == "Adlucent");
            var academy = context.Companies.FirstOrDefault(c => c.Name == "Academy Sports & Outdoors");

            // check that companies have been seeded and retrieved properly
            if (accenture == null || shell == null || deloitte == null || capitalOne == null ||
                ti == null || hilton == null || adlucent == null || academy == null)
            {
                throw new Exception("One or more companies not found. Please seed companies before seeding recruiters.");
            }

            // Seed Recruiters
            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "michelle@example.com",
                    Email = "michelle@example.com",
                    FirstName = "Michelle",
                    LastName = "Banks",
                    Company = accenture,
                    Status = Status.Active
                },
                Password = "jVb0Z6",
                RoleName = "Recruiter"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "elowe@netscrape.net",
                    Email = "elowe@netscrape.net",
                    FirstName = "Ernest",
                    LastName = "Lowe",
                    Company = shell,
                    Status = Status.Active
                },
                Password = "v3n5AV",
                RoleName = "Recruiter"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "mclarence@aool.com",
                    Email = "mclarence@aool.com",
                    FirstName = "Clarence",
                    LastName = "Martin",
                    Company = deloitte,
                    Status = Status.Active
                },
                Password = "zBLq3S",
                RoleName = "Recruiter"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "nelson.Kelly@aool.com",
                    Email = "nelson.Kelly@aool.com",
                    FirstName = "Kelly",
                    LastName = "Nelson",
                    Company = deloitte,
                    Status = Status.Active
                },
                Password = "FSb8rA",
                RoleName = "Recruiter"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "sheff44@ggmail.com",
                    Email = "sheff44@ggmail.com",
                    FirstName = "Martin",
                    LastName = "Sheffield",
                    Company = ti,
                    Status = Status.Active
                },
                Password = "4XKLsd",
                RoleName = "Recruiter"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "yhuik9.Taylor@aool.com",
                    Email = "yhuik9.Taylor@aool.com",
                    FirstName = "Rachel",
                    LastName = "Taylor",
                    Company = hilton,
                    Status = Status.Active
                },
                Password = "9yhFS3",
                RoleName = "Recruiter"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "taylordjay@aool.com",
                    Email = "taylordjay@aool.com",
                    FirstName = "Allison",
                    LastName = "Taylor",
                    Company = adlucent,
                    Status = Status.Active
                },
                Password = "Vjb1wI",
                RoleName = "Recruiter"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "smartinmartin.Martin@aool.com",
                    Email = "smartinmartin.Martin@aool.com",
                    FirstName = "Gregory",
                    LastName = "Martinez",
                    Company = capitalOne,
                    Status = Status.Active
                },
                Password = "1rKkMW",
                RoleName = "Recruiter"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "tanner@ggmail.com",
                    Email = "tanner@ggmail.com",
                    FirstName = "Jeremy",
                    LastName = "Tanner",
                    Company = shell,
                    Status = Status.Active
                },
                Password = "w9wPff",
                RoleName = "Recruiter"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "tee_frank@hootmail.com",
                    Email = "tee_frank@hootmail.com",
                    FirstName = "Frank",
                    LastName = "Tee",
                    Company = academy,
                    Status = Status.Active
                },
                Password = "1EIwbx",
                RoleName = "Recruiter"
            });

            // CSO Seed data
            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser() { UserName = "ra@aoo.com", Email = "ra@aoo.com", FirstName = "Allen", LastName = "Rogers", Status = Status.Active },
                Password = "3wCynC",
                RoleName = "CSO"
            });
            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser() { UserName = "captain@enterprise.net", Email = "captain@enterprise.net", FirstName = "Jean Luc", LastName = "Picard", Status = Status.Active },
                Password = "Pbon0r",
                RoleName = "CSO"
            });

            //create flag to help with errors
            String errorFlag = "Start";

            //create an identity result
            IdentityResult result = new IdentityResult();
            //call the method to seed the user
            try
            {
                foreach (AddUserModel aum in AllUsers)
                {
                    errorFlag = aum.User.Email;
                    result = await Utilities.AddUser.AddUserWithRoleAsync(aum, userManager, context);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("There was a problem adding the user with email: "
                    + errorFlag, ex);
            }

            return result;
        }
    }
}
