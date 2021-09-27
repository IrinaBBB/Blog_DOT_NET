using Blog.Entities;
using Blog.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Blog.Data.Repositories
{
    public class CommentRepository : Repository<Comment>, ICommentRepository
    {
        public CommentRepository(DbContext context) : base(context) { }

        public ApplicationDbContext ApplicationDbContext => Context as ApplicationDbContext;
    }
}