using Application.Models;
using MediatR;

namespace Application.CQRS.Notices
{
    public class UpsertNoticeLinkCommand : IRequest<CommandResponse>
    {
        public UpsertNoticeLinkCommand(List<string> links, Guid noticeId)
        {
            Links = links;
            NoticeId = noticeId;
        }

        public List<String> Links { get; set; }
        public Guid NoticeId { get; set; }
    }
}
