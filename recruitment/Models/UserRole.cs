using Microsoft.AspNetCore.Identity;

namespace recruitment.Models
{
        public class UserRoles
        {
            public string UserId { get; set; }
            public List<IdentityRole> AvailableRoles { get; set; }
            public List<string> SelectedRoles { get; set; }
        }
}
