using Application.Models;
using MediatR;

namespace Application.CQRS.Resources.Translators
{
    public class RemoveTranslatorCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
    }
}
