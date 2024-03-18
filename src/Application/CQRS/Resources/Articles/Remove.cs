using Application.Models;
using MediatR;

namespace Application.CQRS.Resources.Articles
{
    public class RemoveArticleCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
    }

}
