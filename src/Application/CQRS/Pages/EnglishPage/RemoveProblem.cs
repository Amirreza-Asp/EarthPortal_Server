using Application.Models;
using MediatR;

namespace Application.CQRS.Pages.EnglishPage
{
    public class EnglishPageRemoveProblemCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
    }
}
