using Application.Contracts.Infrastructure.Services;
using Application.Contracts.Persistence.Repositories;
using Application.CQRS.Pages.EnglishPage;
using Application.Models;
using Application.Queries;
using Domain;
using Domain.Dtos.Pages;
using Endpoint.CustomeAttributes;
using Endpoint.Utilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Endpoint.Controllers.Pages
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnglishPageController : ControllerBase
    {
        private readonly IEnglishPageRepository _repo;
        private readonly IWebHostEnvironment _hostEnv;
        private readonly IPhotoManager _photoManager;
        private readonly IMediator _mediator;

        public EnglishPageController(IEnglishPageRepository repo, IPhotoManager photoManager, IWebHostEnvironment hostEnv, IMediator mediator)
        {
            _repo = repo;
            _photoManager = photoManager;
            _hostEnv = hostEnv;
            _mediator = mediator;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<EnglishPageDto> Get(CancellationToken cancellationToken) =>
           await _repo.GetAsync(cancellationToken);

        [HttpGet]
        [Route("[action]")]
        public async Task<FileResult> CurrentSituationImage([FromQuery] ImageQuery query, CancellationToken cancellationToken)
        {
            string upload = _hostEnv.WebRootPath;
            string path = $"{upload}{SD.EnglishPageCurrentSituationImage}{query.Name}";
            var image = await _photoManager.ResizeAsync(path, query.Width, query.Height, cancellationToken);

            string extension = Path.GetExtension(query.Name);
            return File(image, $"image/{extension.Substring(1)}");
        }

        [HttpPut]
        [Route("[action]")]
        [AccessControl(SD.AdminRole)]
        public async Task<CommandResponse> ChangeCurrentSituationImage([FromForm] UpdateCurrentSituationImageCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);

        [HttpPut]
        [Route("[action]")]
        [AccessControl(SD.AdminRole)]
        public async Task<CommandResponse> Update([FromBody] UpdateEnglishPageCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);


        [HttpPost]
        [Route("[action]")]
        [AccessControl(SD.AdminRole)]
        public async Task<CommandResponse> AddCard([FromBody] CreateEnglishCardCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);

        [HttpPut]
        [Route("[action]")]
        [AccessControl(SD.AdminRole)]
        public async Task<CommandResponse> UpdateCard([FromBody] UpdateEnglishCardCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);

        [HttpDelete]
        [Route("[action]")]
        [AccessControl(SD.AdminRole)]
        public async Task<CommandResponse> RemoveCard([FromQuery] RemoveEnglishCardCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);

        [HttpPost]
        [Route("[action]")]
        [AccessControl(SD.AdminRole)]
        public async Task<CommandResponse> AddProblem([FromBody] EnglishPageAddProblemCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);

        [HttpPost]
        [Route("[action]")]
        [AccessControl(SD.AdminRole)]
        public async Task<CommandResponse> AddSolution([FromBody] EnglishPageAddSolutionCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);

        [HttpPut]
        [Route("[action]")]
        [AccessControl(SD.AdminRole)]
        public async Task<CommandResponse> UpdateProblem([FromBody] EnglishPageUpdateProblemCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);

        [HttpPut]
        [Route("[action]")]
        [AccessControl(SD.AdminRole)]
        public async Task<CommandResponse> UpdateSolution([FromBody] EnglishPageUpdateSolutionCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);

        [HttpDelete]
        [Route("[action]")]
        [AccessControl(SD.AdminRole)]
        public async Task<CommandResponse> RemoveProblem([FromQuery] EnglishPageRemoveProblemCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);

        [HttpDelete]
        [Route("[action]")]
        [AccessControl(SD.AdminRole)]
        public async Task<CommandResponse> RemoveSolution([FromQuery] EnglishPageRemoveSolutionCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);
    }
}
