using Application.Models;
using MediatR;

namespace Application.CQRS.Resources.Authors
{
    public class RemoveAuthorCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
    }
}
