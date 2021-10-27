using System.Collections.Generic;
using Blog.Entities;

namespace Blog.Models.TagViewModels
{
    public class CreateTagViewModel
    {
        public Tag Tag { get; set; }
        public IEnumerable<Tag> AllTagsList { get; set; }
    }
}