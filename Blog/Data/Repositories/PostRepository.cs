using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Entities;
using Blog.Interfaces.IRepositories;
using Blog.Models.PostViewModels;
using Microsoft.EntityFrameworkCore;

namespace Blog.Data.Repositories
{
    public class PostRepository : Repository<Post>, IPostRepository
    {
        public PostRepository(DbContext context) : base(context) { }

        public ApplicationDbContext ApplicationDbContext => Context as ApplicationDbContext;

        public async Task<CreateEditPostViewModel> GetCreateNewPostViewModelBy(string ownerId)
        {
            var usersBlogs = await ApplicationDbContext.Blogs
                .Where(b => b.OwnerId == new Guid(ownerId)).ToListAsync();

            var viewModel = new CreateEditPostViewModel
            {
                Post = new Post(),
                UsersBlogs = usersBlogs
            };

            return viewModel;
        }

        public async Task<IEnumerable<Post>> GetBlogsPosts(string blogId)
        {
            return await  ApplicationDbContext
                .Posts
                .Where(p => p.BlogId == blogId).ToListAsync();
        }
    }
}