﻿using Application.Contracts.Infrastructure.Services;
using Application.Contracts.Persistence.Repositories;
using Application.CQRS.Multimedia.Gallery;
using Application.Models;
using Application.Queries;
using Domain;
using Domain.Dtos.Multimedia;
using Domain.Entities.Mutimedia;
using Endpoint.CustomeAttributes;
using Endpoint.Utilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Endpoint.Controllers.Multimedia
{
    [Route("api/[controller]")]
    [ApiController]
    public class GalleryController : ControllerBase
    {
        private readonly IRepository<Gallery> _repo;
        private readonly IPhotoManager _photoManager;
        private readonly IWebHostEnvironment _hostEnv;
        private readonly IMediator _mediator;

        public GalleryController(IRepository<Gallery> repo, IWebHostEnvironment hostEnv, IPhotoManager photoManager, IMediator mediator)
        {
            _repo = repo;
            _hostEnv = hostEnv;
            _photoManager = photoManager;
            _mediator = mediator;
        }


        [HttpPost]
        [Route("PagenationSummary")]
        public async Task<ListActionResult<GallerySummary>> PagenationSummary([FromBody] GridQuery query, CancellationToken cancellationToken)
        {
            return await _repo.GetAllAsync<GallerySummary>(query, cancellationToken: cancellationToken);
        }

        [Route("Image")]
        [HttpGet]
        public async Task<IActionResult> GetImage([FromQuery] ImageQuery query, CancellationToken cancellationToken)
        {
            string upload = _hostEnv.WebRootPath;
            string path = $"{upload}{SD.GalleryPath}{query.Name}";
            var image = await _photoManager.ResizeAsync(path, query.Width, query.Height, cancellationToken);

            string extension = Path.GetExtension(query.Name);
            return File(image, $"image/{extension.Substring(1)}");
        }


        [HttpPut]
        [AccessControl(SD.AdminRole)]
        [Route("SetPhotoOrder")]
        public async Task<CommandResponse> SetPhotoOrder([FromBody] SetPhotoOrderCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);

        [HttpPost]
        [AccessControl(SD.AdminRole)]
        [Route("Create")]
        public async Task<CommandResponse> Create([FromForm] CreateGalleryCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.HandleRequestAsync(command, cancellationToken);
        }

        [HttpPut]
        [AccessControl(SD.AdminRole)]
        [Route("Update")]
        public async Task<CommandResponse> Update([FromForm] UpdateGalleryCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);

        [HttpDelete]
        [AccessControl(SD.AdminRole)]
        [Route("Remove")]
        public async Task<CommandResponse> Remove([FromQuery] RemoveGalleryCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);

    }
}
