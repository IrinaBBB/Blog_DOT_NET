using System;

namespace Blog.Entities
{
    public class BlogApplicationUser
    {
        public string OwnerId { get; set; }
        public ApplicationUser Owner { get; set; }
        public Guid BlogId { get; set; }
        public Blog Blog { get; set; }
    }
}