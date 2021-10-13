using System;
using System.ComponentModel.DataAnnotations;

namespace Blog.Models.BlogViewModels
{
    public class CreateBlogViewModel
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid OwnerId { get; set; }
    }
}