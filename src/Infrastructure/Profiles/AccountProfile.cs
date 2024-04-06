using AutoMapper;
using Domain.Dtos.Account;
using Domain.Entities.Account;

namespace Infrastructure.Profiles
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<User, User>();
            CreateMap<Role, Role>();
            CreateMap<User, UserSummary>()
                .ForMember(b => b.Role, d => d.MapFrom(e => e.Role.Display));
        }
    }
}
