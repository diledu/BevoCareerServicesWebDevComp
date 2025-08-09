using Group_11.DAL;
using Group_11.Models;

namespace Group_11.Seeding
{
    public class SeedCompanies
    {
        public static void SeedCompanyData(AppDbContext context)
        {
            if (!context.Companies.Any())
            {
                // get industries
                var accounting = context.Industries.FirstOrDefault(i => i.IndustryName == "Accounting");
                var consulting = context.Industries.FirstOrDefault(i => i.IndustryName == "Consulting");
                var energy = context.Industries.FirstOrDefault(i => i.IndustryName == "Energy");
                var chemicals = context.Industries.FirstOrDefault(i => i.IndustryName == "Chemicals");
                var tech = context.Industries.FirstOrDefault(i => i.IndustryName == "Technology");
                var finance = context.Industries.FirstOrDefault(i => i.IndustryName == "Financial Services");
                var manufacturing = context.Industries.FirstOrDefault(i => i.IndustryName == "Manufacturing");
                var hospitality = context.Industries.FirstOrDefault(i => i.IndustryName == "Hospitality");
                var marketing = context.Industries.FirstOrDefault(i => i.IndustryName == "Marketing");
                var retail = context.Industries.FirstOrDefault(i => i.IndustryName == "Retail");

                // check that industries have been seeded and pulled properly
                if (accounting == null || consulting == null || tech == null || finance == null)
                    throw new Exception("Industries not found. Please run SeedIndustryData first.");

                List<Company> companies = new List<Company>
            {
                new Company
                {
                    Name = "Accenture",
                    Email = "accenture@example.com",
                    Description = "Accenture is a global management consulting, technology services and outsourcing company.",
                    Industries = new List<Industry> { consulting, tech }
                },
                new Company
                {
                    Name = "Shell",
                    Email = "shell@example.com",
                    Description = "Shell Oil Company, including its consolidated companies and its share in equity companies, is one of America's leading oild and natural gas producers, natural gas marketers, gasoline marketers and petrochemical manufacturers.",
                    Industries = new List<Industry> { energy, chemicals }
                },
                new Company
                {
                    Name = "Deloitte",
                    Email = "deloitte@example.com",
                    Description = "Deloitte is one of the leading professional services organizations in the United States specializing in audit, tax, consulting, and financial advisory services with clients in more than 20 industries.",
                    Industries = new List<Industry> { accounting, consulting, tech }
                },
                new Company
                {
                    Name = "Capital One",
                    Email = "capitalone@example.com",
                    Description = "Capital One offers a broad spectrum of financial products and services to consumers, small businesses and commercial clients.",
                    Industries = new List<Industry> { finance }
                },
                new Company
                {
                    Name = "Texas Instruments",
                    Email = "texasinstruments@example.com",
                    Description = "TI is one of the world’s largest global leaders in analog and digital semiconductor design and manufacturing",
                    Industries = new List<Industry> { manufacturing }
                },
                new Company
                {
                    Name = "Hilton Worldwide",
                    Email = "hiltonworldwide@example.com",
                    Description = "Hilton Worldwide offers business and leisure travelers the finest in accommodations, service, amenities and value.",
                    Industries = new List<Industry> { hospitality }
                },
                new Company
                {
                    Name = "Adlucent",
                    Email = "adlucent@example.com",
                    Description = "Adlucent is a technology and analytics company specializing in selling products through the Internet for online retail clients.",
                    Industries = new List<Industry> { marketing, tech }
                },
                new Company
                {
                    Name = "Academy Sports & Outdoors",
                    Email = "academysports@example.com",
                    Description = "Academy Sports is intensely focused on being a leader in the sporting goods, outdoor and lifestyle retail arena",
                    Industries = new List<Industry> { retail }
                }
            };

                context.Companies.AddRange(companies);
                context.SaveChanges();
            }
        }
    }
}
