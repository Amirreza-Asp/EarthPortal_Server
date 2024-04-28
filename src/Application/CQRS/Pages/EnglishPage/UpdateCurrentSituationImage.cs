using Application.Models;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.CQRS.Pages.EnglishPage
{
    public class UpdateCurrentSituationImageCommand : IRequest<CommandResponse>
    {
        public IFormFile Image { get; set; }
    }
}
