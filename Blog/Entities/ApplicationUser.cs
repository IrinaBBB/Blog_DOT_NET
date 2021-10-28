using System;
using Microsoft.AspNetCore.Identity;

namespace Blog.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string DisplayName { get; set; }

    }
}