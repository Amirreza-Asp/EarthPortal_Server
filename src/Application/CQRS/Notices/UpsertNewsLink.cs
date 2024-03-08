using Application.Models;
using MediatR;

namespace Application.CQRS.Notices
{
    public class UpsertNewsLinkCommand : IRequest<CommandResponse>
    {
        public UpsertNewsLinkCommand(List<string> links, Guid newsId)
        {
            Links = links;
            NewsId = newsId;
        }

        public List<String> Links { get; set; }
        public Guid NewsId { get; set; }
    }
}
