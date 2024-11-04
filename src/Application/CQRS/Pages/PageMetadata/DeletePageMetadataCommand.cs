using Application.Models;
using MediatR;

namespace Application.CQRS.Pages.PageMetadata
{
    public class DeletePageMetadataCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
    }
}
