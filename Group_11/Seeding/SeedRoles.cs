using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Group_11.Seeding
{
    public class SeedRoles
    {
        public static async Task AddAllRoles(RoleManager<IdentityRole> roleManager)
        {
            // Add CSO Role
            if (await roleManager.RoleExistsAsync("CSO") == false)
            {
                await roleManager.CreateAsync(new IdentityRole("CSO"));
            }

            // Add Recruiter Role
            if (await roleManager.RoleExistsAsync("Recruiter") == false)
            {
                await roleManager.CreateAsync(new IdentityRole("Recruiter"));
            }

            // Add Recruiter Role
            if (await roleManager.RoleExistsAsync("Student") == false)
            {
                await roleManager.CreateAsync(new IdentityRole("Student"));
            }

        }
    }
}
