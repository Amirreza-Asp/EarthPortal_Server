using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Contact.RelatedLinks;
using Application.Models;
using Domain.Entities.Contact;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Contact.RelatedLinks
{
    public class CreateRelatedLinkCommandHandler : IRequestHandler<CreateRelatedLinkCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CreateRelatedLinkCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;


        public CreateRelatedLinkCommandHandler(ApplicationDbContext context, ILogger<CreateRelatedLinkCommandHandler> logger, IUserAccessor userAccessor)
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(CreateRelatedLinkCommand request, CancellationToken cancellationToken)
        {
            var entity = new RelatedLink(request.Title, request.Link, request.Order);

            _context.RelatedLink.Add(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _logger.LogInformation($"RelatedLink with id {entity.Id} created by {_userAccessor.GetUserName()} in {DateTime.Now}");
                return CommandResponse.Success(entity.Id);
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
