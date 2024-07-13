using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Regulation.Categories;
using Application.Models;
using Domain.Entities.Regulation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Regulation.Categories
{
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CreateCategoryCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public CreateCategoryCommandHandler(ApplicationDbContext context, ILogger<CreateCategoryCommandHandler> logger, IUserAccessor userAccessor)
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var entity = new LawCategory(request.Title);
            entity.Order = request.Order;
            _context.LawCategory.Add(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {

                _logger.LogInformation($"LawCategory with id {entity.Id} created by {_userAccessor.GetUserName()} in {DateTime.Now}");
                return CommandResponse.Success(entity.Id);
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
