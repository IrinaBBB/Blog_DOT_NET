using System;
using System.Collections.Generic;

namespace Blog.Entities
{
    public class Blog
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid OwnerId { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime Updated { get; set; }
        public List<Post> Comments { get; set; }
    }
}