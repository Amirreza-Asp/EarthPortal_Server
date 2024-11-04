using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Pages.PageMetadata;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Pages.PageMetadatas
{
    public class DeletePageMetadataCommandHandler : IRequestHandler<DeletePageMetadataCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CreatePageMetadataCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public DeletePageMetadataCommandHandler(ApplicationDbContext context, ILogger<CreatePageMetadataCommandHandler> logger, IUserAccessor userAccessor)
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(DeletePageMetadataCommand request, CancellationToken cancellationToken)
        {
            var pageMeta = await _context.PageMetadata.Where(b => b.Id == request.Id).FirstOrDefaultAsync(cancellationToken);

            if (pageMeta == null)
                return CommandResponse.Failure(400, "شناسه وارد شده اشتباه است");

            _context.PageMetadata.Remove(pageMeta);

            await _context.SaveChangesAsync();

            _logger.LogInformation($"PageMetadata with id {pageMeta.Id} deleted by {_userAccessor.GetUserName()} in {DateTime.Now}");

            return CommandResponse.Success();
        }
    }
}
