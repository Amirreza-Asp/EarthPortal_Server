using Application.Models;
using MediatR;

namespace Application.CQRS.Regulation.Categories
{
    public class UpdateCategoryCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
        public String Title { get; set; }
        public int Order { get; set; }
    }
}
