using System.Collections.Generic;
using Blog.Entities;

namespace Blog.Models
{
    public class MainPageViewModel
    {
        public IEnumerable<Post> Posts { get; set; }
        public IEnumerable<Entities.Blog> Blogs { get; set; }
        public IEnumerable<Tag> Tags { get; set; }
    }
}