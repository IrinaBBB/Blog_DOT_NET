using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Blog.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Models.PostViewModels
{
    public class CreateEditPostViewModel
    {
        [Required]
        [BindProperty]
        public Post Post { get; set; }
        public IEnumerable<Entities.Blog> UsersBlogs { get; set; }
        public IEnumerable<Tag> Tags { get; set; }
        public string TagIds { get; set; }
    }
}