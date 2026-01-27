using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Pages.PageMetadata;
using Application.Models;
using Domain.Entities.Pages;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Pages.PageMetadatas
{
    public class UpdatePageMetadataCommandHandler
        : IRequestHandler<UpdatePageMetadataCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UpdatePageMetadataCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public UpdatePageMetadataCommandHandler(
            ApplicationDbContext context,
            ILogger<UpdatePageMetadataCommandHandler> logger,
            IUserAccessor userAccessor
        )
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(
            UpdatePageMetadataCommand request,
            CancellationToken cancellationToken
        )
        {
            var pageMeta = await _context
                .PageMetadata.Where(b => b.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (pageMeta == null)
                return CommandResponse.Failure(400, "شناسه وارد شده اشتباه است");

            pageMeta.Title = request.Title;
            pageMeta.Keywords = request.Keywords.Select(b => new PageMetadataKeywords(b)).ToList();
            pageMeta.Description = request.Description;

            _context.PageMetadata.Update(pageMeta);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "PageMetadata with id {Id} updated by {UserRealName} in {DoneTime}",
                pageMeta.Id,
                _userAccessor.GetUserName(),
                DateTimeOffset.UtcNow
            );

            return CommandResponse.Success();
        }
    }
}
