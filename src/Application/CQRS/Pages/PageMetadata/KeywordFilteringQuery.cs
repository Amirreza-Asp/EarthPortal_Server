using Domain.Dtos.Shared;
using MediatR;

namespace Application.CQRS.Pages.PageMetadata
{
    public class KeywordFilteringQuery : IRequest<List<SelectListItem>>
    {
        public required String Text { get; set; }
    }
}
