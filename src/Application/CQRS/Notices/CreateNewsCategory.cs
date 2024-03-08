using Application.Models;
using MediatR;

namespace Application.CQRS.Notices
{
    public class CreateNewsCategoryCommand : IRequest<CommandResponse>
    {
        public String Title { get; set; }
        public String? Description { get; set; }
    }
}
