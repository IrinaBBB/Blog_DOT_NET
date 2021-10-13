using AutoMapper;
using Blog.Entities;
using Blog.Models.BlogViewModels;
using Blog.Models.CommentViewModels;

namespace Blog.Util
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Entities.Blog, CreateBlogViewModel>();
            CreateMap<CreateBlogViewModel, Entities.Blog>();
            CreateMap<Comment, CommentDto>();
            CreateMap<CommentDto, Comment>();
            CreateMap<CreateCommentDto, Comment>();
        }
    }
}