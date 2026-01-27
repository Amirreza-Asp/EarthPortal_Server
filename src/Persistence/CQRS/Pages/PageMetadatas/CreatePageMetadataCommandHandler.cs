using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Pages.PageMetadata;
using Application.Models;
using Domain.Entities.Pages;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Pages.PageMetadatas
{
    public class CreatePageMetadataCommandHandler
        : IRequestHandler<CreatePageMetadataCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CreatePageMetadataCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public CreatePageMetadataCommandHandler(
            ApplicationDbContext context,
            ILogger<CreatePageMetadataCommandHandler> logger,
            IUserAccessor userAccessor
        )
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(
            CreatePageMetadataCommand request,
            CancellationToken cancellationToken
        )
        {
            if (await _context.PageMetadata.AnyAsync(b => b.Page == request.Page))
                return CommandResponse.Failure(400, "برای صفحه مورد نظر متا وجود دارد");

            var pageMeta = new PageMetadata(
                request.Title,
                request.Description,
                request.Page,
                request.Keywords
            );

            _context.PageMetadata.Add(pageMeta);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "PageMetadata with id {Id} created by {UserRealName} in {DoneTime}",
                pageMeta.Id,
                _userAccessor.GetUserName(),
                DateTimeOffset.UtcNow
            );

            return CommandResponse.Success();
        }
    }
}
