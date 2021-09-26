using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Blog.Entities
{
    public class Blog
    {
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid OwnerId { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime Updated { get; set; }
        public virtual List<Post> Posts { get; set; }
    }
}
