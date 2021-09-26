using System.Collections.Generic;
using System.Threading.Tasks;
using Blog.Entities;

namespace Blog.Interfaces.IRepositories
{
    public interface ICommentRepository : IRepository<Comment> { }
}