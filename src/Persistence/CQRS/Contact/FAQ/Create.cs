using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Contact.FAQ;
using Application.Models;
using Domain.Entities.Contact;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Contact.FAQ
{
    public class CreateFAQCommandHandler : IRequestHandler<CreateFAQCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CreateFAQCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public CreateFAQCommandHandler(ApplicationDbContext context, ILogger<CreateFAQCommandHandler> logger, IUserAccessor userAccessor)
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(CreateFAQCommand request, CancellationToken cancellationToken)
        {
            var faq = new FrequentlyAskedQuestions(request.Title, request.Description);
            faq.Order = request.Order;
            _context.FrequentlyAskedQuestions.Add(faq);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _logger.LogInformation($"FAQ with id {faq.Id} registered by {_userAccessor.GetUserName()} in {DateTime.Now}");
                return CommandResponse.Success(faq.Id);
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
