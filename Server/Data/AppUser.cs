using Microsoft.AspNetCore.Identity;

namespace Server.Data
{
    public class AppUser : IdentityUser
    {
        public IEnumerable<IdentityRole>? Roles { get; set; }
    }
}
