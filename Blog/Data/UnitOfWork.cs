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
            Posts = new PostRepository(_context);
            Comments = new CommentRepository(_context);
            Tags = new TagRepository(_context);
            BlogApplicationUsers = new BlogApplicationRepository(_context);
        }

        public IBlogRepository Blogs { get; private set;  }
        public IPostRepository Posts { get; private set;  }
        public ICommentRepository Comments { get; private set;  }
        public ITagRepository Tags { get; private set;  }
        public IBlogApplicationUserRepository BlogApplicationUsers { get; }

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