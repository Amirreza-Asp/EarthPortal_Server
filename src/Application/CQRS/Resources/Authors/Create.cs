using Application.Models;
using MediatR;

namespace Application.CQRS.Resources.Authors
{
    public class CreateAuthorCommand : IRequest<CommandResponse>
    {
        public String Name { get; set; }
        public int Order { get; set; }
    }
}
