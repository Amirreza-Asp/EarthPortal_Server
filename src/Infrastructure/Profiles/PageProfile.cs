using AutoMapper;
using Domain.Entities.Pages;

namespace Infrastructure.Profiles
{
    public class PageProfile : Profile
    {
        public PageProfile()
        {
            CreateMap<HomePage, HomePage>();
            CreateMap<AboutUsPage, AboutUsPage>();
            CreateMap<LawPage, LawPage>();
        }
    }
}
