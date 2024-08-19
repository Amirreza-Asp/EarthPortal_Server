using Application.Models;
using MediatR;

namespace Application.CQRS.Regulation.Categories
{
    public class RemoveCategoryCommand : IRequest<CommandResponse>, IRegulationCommand
    {
        public Guid Id { get; set; }
    }
}
