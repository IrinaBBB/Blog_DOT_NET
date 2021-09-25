using System.Threading.Tasks;
using Blog.Entities;
using Blog.Models.PostViewModels;

namespace Blog.Interfaces.IRepositories
{
    public interface IPostRepository : IRepository<Post>
    {
        Task<CreateNewPostViewModel> GetCreateNewPostViewModelBy(string ownerId);
    }
}