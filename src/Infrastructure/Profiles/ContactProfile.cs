using AutoMapper;
using Domain.Dtos.Contact;
using Domain.Dtos.Content;
using Domain.Entities.Contact;

namespace Infrastructure.Profiles
{
    public class ContactProfile : Profile
    {
        public ContactProfile()
        {
            CreateMap<FrequentlyAskedQuestions, FrequentlyAskedQuestionsSummary>();

            CreateMap<Guide, GuideSummary>();

            CreateMap<Info, InfoSummary>();
            CreateMap<GeoAddress, GeoAddressSummary>()
                .ForMember(b => b.Cordinates, d => d.MapFrom(e => new List<double> { e.Lat, e.Lon }));

            CreateMap<EducationalVideo, EducationalVideo>();

            CreateMap<RelatedCompany, RelatedCompany>();

            CreateMap<Goal, Goal>();


            CreateMap<AboutUs, AboutUsSummary>()
                .ForMember(b => b.HaveVideo, d => d.MapFrom(e => !String.IsNullOrEmpty(e.Video)));

        }
    }
}
