using AutoMapper;
using Domain.Dtos.Shared;
using Domain.Entities.Mutimedia;

namespace Infrastructure.Profiles
{
    public class SharedProfile : Profile
    {
        public SharedProfile()
        {
            CreateMap<GalleryPhoto, ImageSummary>();
        }
    }
}
