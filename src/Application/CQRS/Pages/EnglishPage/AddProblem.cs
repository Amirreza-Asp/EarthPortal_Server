using Application.Models;
using MediatR;

namespace Application.CQRS.Pages.EnglishPage
{
    public class EnglishPageAddProblemCommand : IRequest<CommandResponse>
    {
        public String Content { get; set; }
    }
}
