using Application.Models;
using Domain.Entities.Pages;
using MediatR;

namespace Application.CQRS.Pages.PageMetadata
{
    public class CreatePageMetadataCommand : IRequest<CommandResponse>
    {
        public String Title { get; set; }
        public String Description { get; set; }
        public List<String> Keywords { get; set; }
        public Page Page { get; set; }
    }
}
