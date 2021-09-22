using Blog.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Blog.Data.Repositories
{
    public class BlogRepository : Repository<Entities.Blog>, IBlogRepository
    {
        public BlogRepository(DbContext context) : base(context) { }

        public ApplicationDbContext ApplicationDbContext => Context as ApplicationDbContext;
    }
}