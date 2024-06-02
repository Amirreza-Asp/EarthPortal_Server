using Application.Models;
using MediatR;

namespace Application.CQRS.Notices
{
    public class UpdateNewsCategoryCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
        public String Title { get; set; }
        public String? Description { get; set; }
        public int Order { get; set; }
    }
}
