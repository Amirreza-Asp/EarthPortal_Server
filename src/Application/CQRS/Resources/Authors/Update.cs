using Application.Models;
using MediatR;

namespace Application.CQRS.Resources.Authors
{
    public class UpdateAuthorCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
        public String Name { get; set; }
    }
}
