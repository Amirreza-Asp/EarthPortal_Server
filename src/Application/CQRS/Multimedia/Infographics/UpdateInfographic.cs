using Application.Models;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.CQRS.Multimedia.Infographics
{
    public class UpdateInfographicCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
        public String Title { get; set; }
        public IFormFile? Image { get; set; }
        public bool IsLandscape { get; set; }
        public int Order { get; set; }
    }
}
