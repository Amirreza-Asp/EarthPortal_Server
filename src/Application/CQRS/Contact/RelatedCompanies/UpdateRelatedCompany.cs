using Application.Models;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.CQRS.Contact.RelatedCompanies
{
    public class UpdateRelatedCompanyCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
        public String Name { get; set; }
        public IFormFile? Image { get; set; }
        public int Order { get; set; }
    }
}
