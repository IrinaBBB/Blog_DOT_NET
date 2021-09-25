using System;
using System.ComponentModel.DataAnnotations;

namespace Blog.Models.BlogViewModels
{
    public class CreateBlogViewModel
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public Guid OwnerId { get; set; }
    }
}