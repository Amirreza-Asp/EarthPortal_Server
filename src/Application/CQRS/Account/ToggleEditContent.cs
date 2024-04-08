using Application.Models;
using MediatR;

namespace Application.CQRS.Account
{
    public class ToggleEditContentCommand : IRequest<CommandResponse>
    {
    }
}
