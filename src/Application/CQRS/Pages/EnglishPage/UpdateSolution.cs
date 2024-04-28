using Application.Models;
using MediatR;

namespace Application.CQRS.Pages.EnglishPage
{
    public class EnglishPageUpdateSolutionCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
        public String Content { get; set; }
    }
}
