﻿using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Contact.RelatedLinks;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Contact.RelatedLinks
{
    public class RemoveRelatedLinkCommandHandler : IRequestHandler<RemoveRelatedLinkCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RemoveRelatedLinkCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public RemoveRelatedLinkCommandHandler(ApplicationDbContext context, ILogger<RemoveRelatedLinkCommandHandler> logger, IUserAccessor userAccessor)
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }


        public async Task<CommandResponse> Handle(RemoveRelatedLinkCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.RelatedLink.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (entity == null)
                return CommandResponse.Failure(400, "پیوند انتخاب شده در سیستم وجود ندارد");

            _context.RelatedLink.Remove(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _logger.LogInformation($"RelatedLink with id {entity.Id} removed by {_userAccessor.GetUserName()} in {DateTime.Now}");
                return CommandResponse.Success();
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
