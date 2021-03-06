using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Blog.Data.Repositories
{
    public class BlogRepository : Repository<Entities.Blog>, IBlogRepository
    {
        public BlogRepository(DbContext context) : base(context) { }

        public ApplicationDbContext ApplicationDbContext => Context as ApplicationDbContext;
        public async Task<IEnumerable<Entities.Blog>> GetBlogsByOwnerId(string ownerId)
        {
            return await ApplicationDbContext.Blogs
                .Where(b => b.OwnerId == new Guid(ownerId))
                .ToListAsync();
        }

        public async Task<Entities.Blog> GetBlogWithPosts(string blogId)
        {
            return await ApplicationDbContext.Blogs
                .FindAsync(new Guid(blogId));
        }

        public async Task<IEnumerable<Entities.Blog>> GetSubscriptionsByUser(string userId)
        {
            var blogs =
                await ApplicationDbContext.BlogApplicationUser
                    .Include(x => x.Blog)
                    .Where(entry => entry.OwnerId == userId)
                    .Select(entry => entry.Blog)
                    .ToListAsync();
            return blogs;
        }
    }
}