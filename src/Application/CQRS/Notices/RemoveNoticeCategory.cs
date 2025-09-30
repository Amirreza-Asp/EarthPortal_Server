using Application.Models;
using MediatR;

namespace Application.CQRS.Notices
{
    public class RemoveNoticeCategoryCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
    }
}
