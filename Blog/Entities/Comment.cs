﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Blog.Entities
{
    public class Comment
    {
        public Guid Id { get; set; }
        [Required]
        public string Text { get; set; }
        public Guid PostId { get; set; }
        public Guid OwnerId { get; set; }
        public virtual Post Post { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime Updated { get; set; }
    }
}