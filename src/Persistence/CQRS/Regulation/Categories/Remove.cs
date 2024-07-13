using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Regulation.Categories;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Regulation.Categories
{
    public class RemoveCategoryCommandHandler : IRequestHandler<RemoveCategoryCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RemoveCategoryCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public RemoveCategoryCommandHandler(ApplicationDbContext context, ILogger<RemoveCategoryCommandHandler> logger, IUserAccessor userAccessor)
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(RemoveCategoryCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.LawCategory.FirstOrDefaultAsync(b => b.Id == request.Id);

            if (entity == null)
                return CommandResponse.Failure(400, " موضوع انتخاب شده در سیستم وجود ندارد");


            _context.LawCategory.Remove(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _logger.LogInformation($"LawCategory with id {entity.Id} removed by {_userAccessor.GetUserName()} in {DateTime.Now}");
                return CommandResponse.Success();
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
