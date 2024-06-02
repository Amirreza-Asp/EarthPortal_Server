using Application.CQRS.Contact.FAQ;
using Application.Models;
using Domain.Entities.Contact;
using MediatR;

namespace Persistence.CQRS.Contact.FAQ
{
    public class CreateFAQCommandHandler : IRequestHandler<CreateFAQCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public CreateFAQCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(CreateFAQCommand request, CancellationToken cancellationToken)
        {
            var faq = new FrequentlyAskedQuestions(request.Title, request.Description);
            faq.Order = request.Order;
            _context.FrequentlyAskedQuestions.Add(faq);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success(faq.Id);

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
