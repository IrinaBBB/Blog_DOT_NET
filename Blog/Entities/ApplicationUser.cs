using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Blog.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string DisplayName { get; set; }
        public ICollection<BlogApplicationUser> BlogApplicationUsers { get; set; }
    }
}
