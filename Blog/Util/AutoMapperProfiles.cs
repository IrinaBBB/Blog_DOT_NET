using AutoMapper;
using Blog.Models.BlogViewModels;

namespace Blog.Util
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Entities.Blog, CreateBlogViewModel>();
            CreateMap<CreateBlogViewModel, Entities.Blog>();
        }
    }
}