using System.ComponentModel.DataAnnotations;

namespace Blog.Models.CommentViewModels
{
    public class DeleteCommentDto
    {
        [Required]
        public string OwnerName { get; set; }
        [Required]
        public string CommentId { get; set; }
    }
}