using Microsoft.AspNetCore.Identity;
using System.Data;

namespace recruitment.Models
{
    public class User : IdentityUser
    {
        internal object FullName;
        internal object Address;

        public Role? Role { get; set; }
        public ICollection<UserApplication> UserApplications { get; set; }

    }
}
