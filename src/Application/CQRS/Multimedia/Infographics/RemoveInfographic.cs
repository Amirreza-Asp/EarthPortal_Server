using Application.Models;
using MediatR;

namespace Application.CQRS.Multimedia.Infographics
{
    public class RemoveInfographicCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
    }
}
