using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Regulation.Categories;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Regulation.Categories
{
    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UpdateCategoryCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public UpdateCategoryCommandHandler(ApplicationDbContext context, ILogger<UpdateCategoryCommandHandler> logger, IUserAccessor userAccessor)
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.LawCategory.FirstOrDefaultAsync(b => b.Id == request.Id);

            if (entity == null)
                return CommandResponse.Failure(400, " موضوع انتخاب شده در سیستم وجود ندارد");

            entity.Order = request.Order;
            entity.Title = request.Title;

            _context.LawCategory.Update(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _logger.LogInformation($"LawCategory with id {entity.Id} updated by {_userAccessor.GetUserName()} in {DateTime.Now}");
                return CommandResponse.Success();
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
