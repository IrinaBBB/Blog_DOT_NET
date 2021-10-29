using System;
using Blog.Interfaces.IRepositories;

namespace Blog.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IBlogRepository Blogs { get; }
        IPostRepository Posts { get; }
        ICommentRepository Comments { get; }
        ITagRepository Tags { get; }
        IBlogApplicationUserRepository BlogApplicationUsers { get; }
        int Complete();
    }
}