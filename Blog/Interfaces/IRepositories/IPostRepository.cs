using System.Collections.Generic;
using System.Threading.Tasks;
using Blog.Entities;
using Blog.Models.PostViewModels;

namespace Blog.Interfaces.IRepositories
{
    public interface IPostRepository : IRepository<Post>
    {
        Task<CreateEditPostViewModel> GetCreateNewPostViewModelBy(string ownerId);
        Task<IEnumerable<Post>> GetBlogsPosts(string blogId);
    }
}