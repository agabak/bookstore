using Microsoft.AspNetCore.Identity;

namespace bookstore.Entities
{
    public class AppUser:IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
