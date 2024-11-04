using Application.Models;
using MediatR;

namespace Application.CQRS.Pages.PageMetadata
{
    public class UpdatePageMetadataCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
        public String Title { get; set; }
        public String Description { get; set; }
        public List<String> Keywords { get; set; }
    }
}
