using Application.Contracts.Persistence.Repositories;
using Application.CQRS.Regulation.Laws;
using Application.Models;
using Application.Queries;
using Domain;
using Domain.Dtos.Regulation;
using Domain.Entities.Resources;
using Endpoint.CustomeAttributes;
using Endpoint.Utilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
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

        public LawController(
            ILawRepository lawRepository,
            IMediator mediator,
            IWebHostEnvironment hostEnv,
            ApplicationDbContext context
        )
        {
            _lawRepository = lawRepository;
            _mediator = mediator;
            _hostEnv = hostEnv;
            _context = context;
        }

        [Route("PagenationSpecificQuery")]
        [HttpPost]
        public async Task<ListActionResult<LawSummary>> PagenationSpecificQuery(
            [FromBody] LawPagenationQuery query,
            CancellationToken cancellationToken
        ) => await _lawRepository.PaginationSummaryAsync(query, cancellationToken);

        [Route("PagenationSummary")]
        [HttpPost]
        public async Task<ListActionResult<LawSummary>> PagenationSummary(
            [FromBody] GridQuery query,
            CancellationToken cancellationToken
        ) =>
            await _lawRepository.GetAllAsync<LawSummary>(
                query,
                cancellationToken: cancellationToken
            );

        [Route("Find")]
        [HttpGet]
        public async Task<LawDetails> Find(
            [FromQuery] Guid id,
            CancellationToken cancellationToken
        ) =>
            await _lawRepository.FirstOrDefaultAsync<LawDetails>(
                b => b.Id == id,
                cancellationToken
            );

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> DownloadFile(
            [FromQuery] Guid id,
            CancellationToken cancellationToken
        )
        {
            var law = await _lawRepository.FirstOrDefaultAsync(b => b.Id == id);

            if (law == null)
                return NotFound();

            var baseDir = Path.GetFullPath(Path.Combine(_hostEnv.WebRootPath, SD.LawPdfPath));

            var safeFileName = Path.GetFileName(law.Pdf);

            var fullPath = Path.GetFullPath(Path.Combine(baseDir, safeFileName));

            if (!fullPath.StartsWith(baseDir, StringComparison.OrdinalIgnoreCase))
                return Forbid();

            if (!System.IO.File.Exists(fullPath))
                return NotFound();

            var downloadName = law.Title + Path.GetExtension(safeFileName);

            return PhysicalFile(fullPath, "application/octet-stream", downloadName);
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> DownloadNewspaperFile(
            [FromQuery] Guid id,
            CancellationToken cancellationToken
        )
        {
            var law = await _lawRepository.FirstOrDefaultAsync(b => b.Id == id);
            string upload = _hostEnv.WebRootPath;
            string path = $"{upload}{SD.LawNewspaperPath}{law.Newspaper?.File}";

            var fileBytes = System.IO.File.ReadAllBytes(path);
            string extension = Path.GetExtension(law.Pdf);
            return File(fileBytes, "application/force-download", law.Title + extension);
        }

        [Route("[action]")]
        [HttpGet]
        public String DownloadFileBase64(
            [FromQuery] String file,
            CancellationToken cancellationToken
        )
        {
            string upload = _hostEnv.WebRootPath;
            string path = $"{upload}{SD.LawPdfPath}{file}";

            var fileBytes = System.IO.File.ReadAllBytes(path);

            return Convert.ToBase64String(fileBytes);
        }

        [Route("Create")]
        [HttpPost]
        [AccessControl(SD.AdminRole, SD.LegalRole)]
        public async Task<CommandResponse> Create(
            [FromForm] CreateLawCommand command,
            CancellationToken cancellationToken
        ) => await _mediator.HandleRequestAsync(command, cancellationToken);

        [Route("Update")]
        [HttpPut]
        [AccessControl(SD.AdminRole, SD.LegalRole)]
        public async Task<CommandResponse> Update(
            [FromForm] UpdateLawCommand command,
            CancellationToken cancellationToken
        ) => await _mediator.HandleRequestAsync(command, cancellationToken);

        [Route("Remove")]
        [HttpDelete]
        [AccessControl(SD.AdminRole, SD.LegalRole)]
        public async Task<CommandResponse> Remove(
            [FromQuery] RemoveLawCommand command,
            CancellationToken cancellationToken
        ) => await _mediator.HandleRequestAsync(command, cancellationToken);
    }
}
