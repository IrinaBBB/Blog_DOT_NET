using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Blog.Entities;
using Microsoft.AspNetCore.Identity;

namespace Blog.Models.CommentViewModels
{
    public class CommentDto
    {
        public Guid Id { get; set; }
        [Required] public string Text { get; set; }
        public Guid PostId { get; set; }
        public Guid OwnerId { get; set; }
        public string OwnerName { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
}