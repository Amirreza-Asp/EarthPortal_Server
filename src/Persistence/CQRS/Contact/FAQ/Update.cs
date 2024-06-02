using Application.CQRS.Contact.FAQ;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Contact.FAQ
{
    public class UpdateFAQCommandHandler : IRequestHandler<UpdateFAQCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public UpdateFAQCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(UpdateFAQCommand request, CancellationToken cancellationToken)
        {
            var faq = await _context.FrequentlyAskedQuestions.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (faq == null)
                return CommandResponse.Failure(400, "سوال انتخاب شده در سیستم وجود ندارد");

            faq.Order = request.Order;
            faq.Title = request.Title;
            faq.Content = request.Description;

            _context.FrequentlyAskedQuestions.Update(faq);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
