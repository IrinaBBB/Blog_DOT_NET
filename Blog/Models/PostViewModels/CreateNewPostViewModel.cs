using System;
using System.Collections.Generic;
using Blog.Entities;

namespace Blog.Models.PostViewModels
{
    public class CreateNewPostViewModel
    {
        public Post Post { get; set; }
        public IEnumerable<Entities.Blog> UsersBlogs { get; set; }
    }
}