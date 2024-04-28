using Application.Models;
using MediatR;

namespace Application.CQRS.Pages.EnglishPage
{
    public class RemoveEnglishCardCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
    }
}
