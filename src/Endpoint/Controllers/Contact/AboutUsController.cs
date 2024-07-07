using Application.Contracts.Infrastructure.Services;
using Application.Contracts.Persistence.Repositories;
using Application.CQRS.Contact.About;
using Application.Models;
using Application.Queries;
using Domain;
using Domain.Dtos.Contact;
using Domain.Entities.Contact;
using Endpoint.CustomeAttributes;
using Endpoint.Utilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Endpoint.Controllers.Contact
{
    [Route("api/[controller]")]
    [ApiController]
    public class AboutUsController : ControllerBase
    {
        private readonly IRepository<AboutUs> _aboutUsRepo;
        private readonly IMediator _mediator;
        private readonly IWebHostEnvironment _hostEnv;
        private readonly IPhotoManager _photoManager;

        public AboutUsController(IRepository<AboutUs> aboutUs, IMediator mediator, IPhotoManager photoManager, IWebHostEnvironment hostEnv)
        {
            _aboutUsRepo = aboutUs;
            _mediator = mediator;
            _photoManager = photoManager;
            _hostEnv = hostEnv;
        }

        [HttpPost]
        [Route("PaginationSummary")]
        public async Task<ListActionResult<AboutUsSummary>> PaginationSummary([FromBody] GridQuery query, CancellationToken cancellationToken) =>
            await _aboutUsRepo.GetAllAsync<AboutUsSummary>(query, cancellationToken: cancellationToken);

        [Route("Image")]
        [HttpGet]
        public async Task<IActionResult> GetImage([FromQuery] ImageQuery query, CancellationToken cancellationToken)
        {
            string upload = _hostEnv.WebRootPath;
            string path = $"{upload}{SD.AboutUsPath}{query.Name}";
            var image = await _photoManager.ResizeAsync(path, query.Width, query.Height, cancellationToken);

            string extension = Path.GetExtension(query.Name);
            return File(image, $"image/{extension.Substring(1)}");
        }

        [Route("Create")]
        [HttpPost]
        [AccessControl("Admin")]
        public async Task<CommandResponse> Create([FromForm] CreateAboutCommand command, CancellationToken cancellationToken) =>
           await _mediator.HandleRequestAsync(command, cancellationToken);

        [Route("Update")]
        [HttpPut]
        [AccessControl("Admin")]
        public async Task<CommandResponse> Update([FromForm] UpdateAboutCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);

        [Route("Remove")]
        [HttpDelete]
        [AccessControl("Admin")]
        public async Task<CommandResponse> Remove([FromQuery] RemoveAboutCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);
    }
}
