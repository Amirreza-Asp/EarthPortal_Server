using AutoMapper;
using Domain.Dtos.Regulation;
using Domain.Dtos.Shared;
using Domain.Entities.Regulation;
using Domain.Entities.Regulation.Enums;

namespace Infrastructure.Profiles
{
    public class RegulationProfile : Profile
    {

        public RegulationProfile()
        {
            CreateMap<Law, Law>();
            CreateMap<Law, LawSummary>()
                .ForMember(b => b.Type, d => d.MapFrom(e => e.Type == LawType.Rule ? "آیین نامه" : "قانون"))
                .ForMember(b => b.ApprovalAuthority, d => d.MapFrom(e => e.ApprovalAuthority.Name));
            CreateMap<Law, LawDetails>()
                .ForMember(b => b.ApprovalAuthority, d => d.MapFrom(e => e.ApprovalAuthority.Name))
                .ForMember(b => b.Type, d => d.MapFrom(e => e.Type == LawType.Rule ? "آیین نامه" : "قانون"))
                .ForMember(b => b.ApprovalStatusTitle, d => d.MapFrom(e => e.ApprovalStatus.Status))
                .ForMember(b => b.ApprovalAuthority, d => d.MapFrom(e => e.ApprovalAuthority.Name))
                .ForMember(b => b.ApprovalTypeTitle, d => d.MapFrom(e => e.ApprovalType.Value))
                .ForMember(b => b.ExecutorManagmentTitle, d => d.MapFrom(e => e.ExecutorManagment.Name))
                .ForMember(b => b.LawCategoryTitle, d => d.MapFrom(e => e.LawCategory.Title));


            CreateMap<LawCategory, SelectListItem>()
                .ForMember(b => b.Value, d => d.MapFrom(e => e.Id))
                .ForMember(b => b.Text, d => d.MapFrom(e => e.Title));

            CreateMap<ApprovalAuthority, SelectListItem>()
                .ForMember(b => b.Value, d => d.MapFrom(e => e.Id))
                .ForMember(b => b.Text, d => d.MapFrom(e => e.Name));

            CreateMap<ApprovalStatus, SelectListItem>()
                .ForMember(b => b.Value, d => d.MapFrom(e => e.Id))
                .ForMember(b => b.Text, d => d.MapFrom(e => e.Status));

            CreateMap<ApprovalType, SelectListItem>()
                .ForMember(b => b.Value, d => d.MapFrom(e => e.Id))
                .ForMember(b => b.Text, d => d.MapFrom(e => e.Value));

            CreateMap<ExecutorManagment, SelectListItem>()
                .ForMember(b => b.Value, d => d.MapFrom(e => e.Id))
                .ForMember(b => b.Text, d => d.MapFrom(e => e.Name));
        }

    }
}
