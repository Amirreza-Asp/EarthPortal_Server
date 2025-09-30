using AutoMapper;
using Domain.Dtos.Notices;
using Domain.Entities.Notices;

namespace Infrastructure.Profiles
{
    public class NoticesProfile : Profile
    {
        public NoticesProfile()
        {
            CreateMap<News, NewsSummary>()
                .ForMember(b => b.Image, d => d.MapFrom(e => e.Images.First().Name));

            CreateMap<Notice, NoticeSummary>()
                .ForMember(b => b.Image, d => d.MapFrom(e => e.Images.First().Name));

            CreateMap<News, NewsDetails>()
                .ForMember(b => b.Image, d => d.MapFrom(e => e.Images.First().Name))
                .ForMember(b => b.Keywords, d => d.MapFrom(e => e.Links.Select(e => e.Link)));

            CreateMap<Notice, NoticeDetails>()
                .ForMember(b => b.Image, d => d.MapFrom(e => e.Images.First().Name))
                .ForMember(b => b.Keywords, d => d.MapFrom(e => e.Links.Select(e => e.Link)));

            CreateMap<NewsCategory, NewsCategorySummary>();
            CreateMap<NoticeCategory, NoticeCategorySummary>();

            CreateMap<NewsCategory, NewsCategory>();
            CreateMap<NoticeCategory, NoticeCategory>();

            CreateMap<Link, Keyword>();
        }
    }
}
