using Blog.Entities;
using Blog.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Blog.Data.Repositories
{
    public class BlogApplicationRepository : Repository<BlogApplicationUser>, IBlogApplicationUserRepository
    {
        public BlogApplicationRepository(DbContext context) : base(context)
        {
        }
    }
}