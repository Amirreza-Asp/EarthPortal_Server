using Application.Contracts.Infrastructure.Services;
using Application.Contracts.Persistence.Repositories;
using Application.CQRS.Contact.RelatedLinks;
using Application.Models;
using Application.Queries;
using Domain;
using Domain.Entities.Contact;
using Endpoint.Utilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Endpoint.Controllers.Contact
{
    [Route("api/[controller]")]
    [ApiController]
    public class RelatedLinkController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IWebHostEnvironment _hostEnv;
        private readonly IRepository<RelatedLink> _repo;
        private readonly IPhotoManager _photoManager;

        public RelatedLinkController(IMediator mediator, IRepository<RelatedLink> repo, IWebHostEnvironment hostEnv, IPhotoManager photoManager)
        {
            _mediator = mediator;
            _repo = repo;
            _hostEnv = hostEnv;
            _photoManager = photoManager;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ListActionResult<RelatedLink>> PaginationSummary([FromBody] GridQuery query, CancellationToken cancellationToken) =>
            await _repo.GetAllAsync<RelatedLink>(query, cancellationToken: cancellationToken);


        [HttpPost]
        [Route("[action]")]
        public async Task<CommandResponse> Create([FromBody] CreateRelatedLinkCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);

        [HttpPut]
        [Route("[action]")]
        public async Task<CommandResponse> Update([FromBody] UpdateRelatedLinkCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);


        [HttpDelete]
        [Route("[action]")]
        public async Task<CommandResponse> Remove([FromQuery] RemoveRelatedLinkCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);


        [HttpGet]
        [Route("[action]")]
        public async Task<FileResult> FooterImage(CancellationToken cancellationToken)
        {
            var upload = _hostEnv.WebRootPath + SD.FooterImagePath;
            var files = Directory.GetFiles(upload);

            if (!files.Any())
                throw new Exception("footer image not found");

            var image = await _photoManager.ResizeAsync(files[0], 700, 700, cancellationToken);

            string extension = Path.GetExtension(files[0]);
            return File(image, $"image/{extension.Substring(1)}");
        }

        [HttpPut]
        [Route("[action]")]
        public async Task<CommandResponse> ReplaceFooterImage([FromForm] ReplaceFooterImageCommand command, CancellationToken cancellationToken)
        {
            if (command.Image == null)
                return CommandResponse.Failure(400, "عکس را وارد کنید");

            var upload = _hostEnv.WebRootPath + SD.FooterImagePath;
            var files = Directory.GetFiles(upload);

            foreach (var file in files)
                System.IO.File.Delete(file);

            _photoManager.Save(command.Image, upload + command.Image.FileName);

            return CommandResponse.Success();
        }
    }

    public class ReplaceFooterImageCommand
    {
        public IFormFile Image { get; set; }
    }
}
