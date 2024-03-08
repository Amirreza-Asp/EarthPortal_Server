using AutoMapper;
using Domain.Entities.Account;

namespace Infrastructure.Profiles
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<User, User>();
        }
    }
}
