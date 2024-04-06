using Application.Models;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.CQRS.Contact.RelatedCompanies
{
    public class CreateRelatedCompanyCommand : IRequest<CommandResponse>
    {
        public String Name { get; set; }
        public IFormFile Image { get; set; }
        public int Order { get; set; }
        public String Link { get; set; }
    }
}
