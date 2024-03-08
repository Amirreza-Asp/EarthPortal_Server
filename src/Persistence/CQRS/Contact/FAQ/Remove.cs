using Application.CQRS.Contact.FAQ;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Contact.FAQ
{
    public class RemoveFAQCommandHandler : IRequestHandler<RemoveFAQCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public RemoveFAQCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(RemoveFAQCommand request, CancellationToken cancellationToken)
        {
            var faq = await _context.FrequentlyAskedQuestions.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);


            if (faq == null)
                return CommandResponse.Failure(400, "سوال انتخاب شده در سیستم وجود ندارد");

            _context.FrequentlyAskedQuestions.Remove(faq);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
