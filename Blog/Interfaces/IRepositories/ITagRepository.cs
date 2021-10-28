using System.Collections.Generic;
using System.Threading.Tasks;
using Blog.Entities;

namespace Blog.Interfaces.IRepositories
{
    public interface ITagRepository : IRepository<Tag>
    {
        Task<IEnumerable<Post>> GetPostsByTag(string tagId);
    }
}
