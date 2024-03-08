using AutoMapper;
using Domain.Dtos.Multimedia;
using Domain.Entities.Mutimedia;

namespace Infrastructure.Profiles
{
    public class MultimediaProfile : Profile
    {

        public MultimediaProfile()
        {
            CreateMap<Gallery, GallerySummary>()
                .ForMember(b => b.Images, d => d.MapFrom(e => e.Images.OrderBy(o => o.Order)));

            CreateMap<VideoContent, VideoContentSummary>();

            CreateMap<Infographic, Infographic>();
        }

    }
}
