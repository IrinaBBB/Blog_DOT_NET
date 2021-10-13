using System;
using System.Threading.Tasks;
using Blog.Entities;
using Microsoft.AspNetCore.SignalR;

namespace Blog.Hubs
{
    public class CommentsHub : Hub
    {
        public async Task AddComment(string text)
        {
            var comment = new Comment
            {
                Text = text, 
                OwnerId = new Guid("5b42a153-142c-429c-97a4-2889f8773ecf"),
                Created = DateTime.Now
            };
            await Clients.All.SendAsync("ReceiveComment", comment.Text, comment.OwnerId, comment.Created);
        }

    }
}