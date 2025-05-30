﻿using Application.Contracts.Persistence.Repositories;
using Application.CQRS.Regulation.Categories;
using Application.Models;
using Application.Queries;
using Domain;
using Domain.Dtos.Shared;
using Domain.Entities.Regulation;
using Endpoint.CustomeAttributes;
using Endpoint.Utilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Endpoint.Controllers.Regulation
{
    [Route("api/[controller]")]
    [ApiController]
    public class LawCategoryController : ControllerBase
    {
        private readonly IRepository<LawCategory> _repo;
        private readonly IMediator _mediator;

        public LawCategoryController(IRepository<LawCategory> repo, IMediator mediator)
        {
            _repo = repo;
            _mediator = mediator;
        }


        [HttpPost]
        [Route("SelectListPagenation")]
        public async Task<ListActionResult<SelectListItem>> SelectListPagenation([FromBody] GridQuery query, CancellationToken cancellationToken) =>
            await _repo.GetAllAsync<SelectListItem>(query, cancellationToken: cancellationToken);

        [HttpPost]
        [Route("Create")]
        [AccessControl(SD.AdminRole, SD.LegalRole)]
        public async Task<CommandResponse> Create([FromBody] CreateCategoryCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);

        [HttpPut]
        [Route("Update")]
        [AccessControl(SD.AdminRole, SD.LegalRole)]
        public async Task<CommandResponse> Update([FromBody] UpdateCategoryCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);

        [HttpDelete]
        [Route("Remove")]
        [AccessControl(SD.AdminRole, SD.LegalRole)]
        public async Task<CommandResponse> Remove([FromQuery] RemoveCategoryCommand command, CancellationToken cancellationToken) =>
            await _mediator.HandleRequestAsync(command, cancellationToken);
    }
}
