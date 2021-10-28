using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Entities;
using Blog.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Blog.Data.Repositories
{
    public class TagRepository : Repository<Tag>, ITagRepository
    {
        public ApplicationDbContext ApplicationDbContext => Context as ApplicationDbContext;
        public TagRepository(DbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Post>> GetPostsByTag(string tagId)
        {
            var posts =  await ApplicationDbContext
                .Tags
                .Where(t => t.Id == new Guid(tagId)).SelectMany(c => c.Posts).ToListAsync();
            return posts; 
        }
    }
}