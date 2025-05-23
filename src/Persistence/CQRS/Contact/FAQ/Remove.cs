﻿using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Contact.FAQ;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Contact.FAQ
{
    public class RemoveFAQCommandHandler : IRequestHandler<RemoveFAQCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RemoveFAQCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public RemoveFAQCommandHandler(ApplicationDbContext context, ILogger<RemoveFAQCommandHandler> logger, IUserAccessor userAccessor)
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(RemoveFAQCommand request, CancellationToken cancellationToken)
        {
            var faq = await _context.FrequentlyAskedQuestions.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);


            if (faq == null)
                return CommandResponse.Failure(400, "سوال انتخاب شده در سیستم وجود ندارد");

            _context.FrequentlyAskedQuestions.Remove(faq);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _logger.LogInformation($"FAQ with id {faq.Id} removed by {_userAccessor.GetUserName()} in {DateTime.Now}");
                return CommandResponse.Success();
            }
            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
