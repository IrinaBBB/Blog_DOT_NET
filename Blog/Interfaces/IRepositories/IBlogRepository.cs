using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blog.Interfaces.IRepositories
{
    public interface IBlogRepository : IRepository<Entities.Blog>
    {
        Task<IEnumerable<Entities.Blog>> GetBlogsByOwnerId(string ownerId);
        Task<Entities.Blog> GetBlogWithPosts(string blogId);

        Task<IEnumerable<Entities.Blog>> GetSubscriptionsByUser(string userId);
    }
}