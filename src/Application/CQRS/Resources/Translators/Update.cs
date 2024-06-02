using Application.Models;
using MediatR;

namespace Application.CQRS.Resources.Translators
{
    public class UpdateTranslatorCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
        public int Order { get; set; }
        public string Name { get; set; }
    }
}
