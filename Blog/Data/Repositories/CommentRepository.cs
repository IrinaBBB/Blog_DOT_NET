using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Entities;
using Blog.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Blog.Data.Repositories
{
    public class CommentRepository : Repository<Comment>, ICommentRepository
    {
        public CommentRepository(DbContext context) : base(context) { }

        public ApplicationDbContext ApplicationDbContext => Context as ApplicationDbContext;

        public async Task<IEnumerable<Comment>> GetCommentsByPostId(string postId)
        {
            return await ApplicationDbContext.Comments
                .Where(c => c.PostId == new Guid(postId))
                .OrderByDescending(c => c.Created)
                .ToListAsync();
        }
    }
}