using System;
using System.Collections.Generic;
using Blog.Entities;

namespace Blog.Models.PostViewModels
{
    public class CreateEditPostViewModel
    {
        public Post Post { get; set; }
        public IEnumerable<Entities.Blog> UsersBlogs { get; set; }
    }
}