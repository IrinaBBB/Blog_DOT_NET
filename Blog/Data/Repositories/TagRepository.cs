using Blog.Entities;
using Blog.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Blog.Data.Repositories
{
    public class TagRepository : Repository<Tag>, ITagRepository
    {
        public TagRepository(DbContext context) : base(context)
        {
        }
    }
}