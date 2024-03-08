using Application.Models;
using MediatR;

namespace Application.CQRS.Notices
{
    public class RemoveNewsCategoryCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
    }
}
