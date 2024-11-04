using AutoMapper;
using Domain.Dtos.Pages;
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
            CreateMap<LawPage, LawPageDto>()
                .ForMember(b => b.LawCount, e => e.Ignore());

            CreateMap<PageMetadata, PageMetadataSummary>()
                .ForMember(b => b.Keywords, e => e.MapFrom(b => b.Keywords.Select(b => b.Value)));

            CreateMap<EnglishCard, EnglishCardDto>()
                .ForMember(b => b.Type, d => d.MapFrom(e => e.Type.ToString()))
                .ForMember(b => b.Color, d => d.MapFrom(e => e.Color.ToString()));
        }
    }
}
