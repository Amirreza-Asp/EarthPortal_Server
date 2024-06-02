using Application.Models;
using MediatR;

namespace Application.CQRS.Resources.Translators
{
    public class CreateTranslatorCommand : IRequest<CommandResponse>
    {
        public int Order { get; set; }
        public string Name { get; set; }
    }
}
