using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Group_11.Models.ViewModels
{
    public class RoleEditModel
    {
        public IdentityRole Role { get; set; }
        public IEnumerable<AppUser> RoleMembers { get; set; }
        public IEnumerable<AppUser> RoleNonMembers { get; set; }
    }

    //NOTE: have to make string arrays nullable, otherwise ModelState always returns false
    public class RoleModificationModel
    {
        [Required]
        public string RoleName { get; set; }
        public string[]? IdsToAdd { get; set; }
        public string[]? IdsToDelete { get; set; }
    }
}