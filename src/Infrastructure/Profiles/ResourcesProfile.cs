using AutoMapper;
using Domain.Dtos.Resources;
using Domain.Dtos.Shared;
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

            CreateMap<Broadcast, BroadcastDetails>()
                .ForMember(b => b.Author, d => d.MapFrom(e => e.Author.FullName))
                .ForMember(b => b.Publication, d => d.MapFrom(e => e.Publication.Title))
                .ForMember(b => b.Translator, d => d.MapFrom(e => e.Translator.FullName));

            CreateMap<Article, ArticleDetails>()
                .ForMember(b => b.Author, d => d.MapFrom(e => e.Author.FullName))
                .ForMember(b => b.Publication, d => d.MapFrom(e => e.Publication.Title))
                .ForMember(b => b.Translator, d => d.MapFrom(e => e.Translator.FullName));

            CreateMap<Broadcast, BroadcastSummary>()
                .ForMember(b => b.Author, d => d.MapFrom(e => e.Author.FullName));

            CreateMap<Article, ArticleSummary>()
                .ForMember(b => b.Author, d => d.MapFrom(e => e.Author.FullName));

            CreateMap<Author, SelectListItem>()
                .ForMember(b => b.Text, d => d.MapFrom(e => e.FullName))
                .ForMember(b => b.Value, d => d.MapFrom(e => e.Id));

            CreateMap<Publication, SelectListItem>()
                .ForMember(b => b.Text, d => d.MapFrom(e => e.Title))
                .ForMember(b => b.Value, d => d.MapFrom(e => e.Id));

            CreateMap<Translator, SelectListItem>()
                .ForMember(b => b.Text, d => d.MapFrom(e => e.FullName))
                .ForMember(b => b.Value, d => d.MapFrom(e => e.Id));



        }
    }
}
