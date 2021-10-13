using System;
using System.ComponentModel.DataAnnotations;

namespace Blog.Models.CommentViewModels
{
    public class CreateCommentDto
    {
        [Required] public string Text { get; set; }
        [Required] public Guid PostId { get; set; }
        public string OwnerId { get; set; }
    }
}