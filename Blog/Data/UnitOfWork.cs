using Blog.Data.Repositories;
using Blog.Interfaces;
using Blog.Interfaces.IRepositories;

namespace Blog.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Blogs = new BlogRepository(_context);
        }

        public IBlogRepository Blogs { get; private set;  }
        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}