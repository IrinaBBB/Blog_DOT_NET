using System;
using System.Collections.Generic;

namespace Blog.Entities
{
    public class Post
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public Guid OwnerId { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime Updated { get; set; }
        public List<Comment> Comments { get; set; }
    }
}