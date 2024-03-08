using Application.Models;
using MediatR;

namespace Application.CQRS.Contact.RelatedCompanies
{
    public class RemoveRelatedCompanyCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
    }
}
