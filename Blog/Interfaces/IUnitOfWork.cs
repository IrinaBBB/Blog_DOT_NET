using System;
using Blog.Interfaces.IRepositories;

namespace Blog.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IBlogRepository Blogs { get; }
        IPostRepository Posts { get; }
        int Complete();
    }
}