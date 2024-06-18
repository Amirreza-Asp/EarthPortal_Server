using Application.Contracts.Persistence.Repositories;
using Application.CQRS.Regulation.Laws;
using Application.Models;
using Application.Queries;
using Domain;
using Domain.Dtos.Regulation;
using Endpoint.CustomeAttributes;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Endpoint.Controllers.Regulation
{
    // preline
    [Route("api/[controller]")]
    [ApiController]
    public class LawController : ControllerBase
    {
        private readonly ILawRepository _lawRepository;
        private readonly IMediator _mediator;
        private readonly IWebHostEnvironment _hostEnv;
        private readonly ApplicationDbContext _context;

        public LawController(ILawRepository lawRepository, IMediator mediator, IWebHostEnvironment hostEnv, ApplicationDbContext context)
        {
            _lawRepository = lawRepository;
            _mediator = mediator;
            _hostEnv = hostEnv;
            _context = context;
        }


        [Route("PagenationSpecificQuery")]
        [HttpPost]
        public async Task<ListActionResult<LawSummary>> PagenationSpecificQuery([FromBody] LawPagenationQuery query, CancellationToken cancellationToken) =>
            await _lawRepository.PagenationSummaryAsync(query, cancellationToken);

        [Route("PagenationSummary")]
        [HttpPost]
        public async Task<ListActionResult<LawSummary>> PagenationSummary([FromBody] GridQuery query, CancellationToken cancellationToken) =>
            await _lawRepository.GetAllAsync<LawSummary>(query, cancellationToken: cancellationToken);

        [Route("Find")]
        [HttpGet]
        public async Task<LawDetails> Find([FromQuery] Guid id, CancellationToken cancellationToken) =>
            await _lawRepository.FirstOrDefaultAsync<LawDetails>(b => b.Id == id, cancellationToken);

        [Route("Test")]
        [HttpDelete]
        public async Task<IActionResult> RemoveRepeatedLaws(CancellationToken cancellationToken)
        {
            var laws = await _context.Law.ToListAsync();
            var okLaws = laws.DistinctBy(s => s.Title.Trim()).ToList();
            var removeLaws = laws.Where(b => !okLaws.Any(s => s.Id == b.Id)).ToList();

            foreach (var item in laws)
            {
                if (laws.Count(s => s.Title == item.Title) > 1 &&
                    okLaws.Select(s => s.Title).Contains(item.Title))
                    removeLaws.Add(item);
                else
                    okLaws.Add(item);
            }


            foreach (var item in removeLaws)
            {
                _lawRepository.Remove(item);
            }


            await _lawRepository.SaveAsync();
            return Ok();
        }


        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> DownloadFile([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            var law = await _lawRepository.FirstOrDefaultAsync(b => b.Id == id);
            string upload = _hostEnv.WebRootPath;
            string path = $"{upload}{SD.LawPdfPath}{law.Pdf}";

            var fileBytes = System.IO.File.ReadAllBytes(path);
            string extension = Path.GetExtension(law.Pdf);
            return File(fileBytes, "application/force-download", law.Title + extension);
        }


        [Route("[action]")]
        [HttpGet]
        public String DownloadFileBase64([FromQuery] String file, CancellationToken cancellationToken)
        {
            string upload = _hostEnv.WebRootPath;
            string path = $"{upload}{SD.LawPdfPath}{file}";

            var fileBytes = System.IO.File.ReadAllBytes(path);

            return Convert.ToBase64String(fileBytes);
        }


        [Route("Create")]
        [HttpPost]
        [AccessControl("Admin")]
        public async Task<CommandResponse> Create([FromForm] CreateLawCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);


        [Route("Update")]
        [HttpPut]
        [AccessControl("Admin")]
        public async Task<CommandResponse> Update([FromForm] UpdateLawCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);


        [Route("Remove")]
        [HttpDelete]
        [AccessControl("Admin")]
        public async Task<CommandResponse> Remove([FromQuery] RemoveLawCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);
    }
}
