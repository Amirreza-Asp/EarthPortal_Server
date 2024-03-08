using AutoMapper;
using Domain.Dtos.Resources;
using Domain.Entities.Resources;

namespace Infrastructure.Profiles
{
    public class ResourcesProfile : Profile
    {
        public ResourcesProfile()
        {
            CreateMap<Book, BookSummary>()
                .ForMember(b => b.Author, d => d.MapFrom(e => e.Author.FullName));

            CreateMap<Book, BookDetails>()
                .ForMember(b => b.Author, d => d.MapFrom(e => e.Author.FullName))
                .ForMember(b => b.Publication, d => d.MapFrom(e => e.Publication.Title))
                .ForMember(b => b.Translator, d => d.MapFrom(e => e.Translator.FullName));

            CreateMap<Broadcast, BroadcastSummary>()
                .ForMember(b => b.Author, d => d.MapFrom(e => e.Author.FullName));

            CreateMap<Article, ArticleSummary>()
                .ForMember(b => b.Author, d => d.MapFrom(e => e.Author.FullName));



        }
    }
}
