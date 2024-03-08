using Application.CQRS.Regulation.Laws;
using Application.Models;
using Domain.Entities.Regulation.Enums;
using Domain.Entities.Regulation.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Regulation.Laws
{
    public class UpdateLawCommandHandler : IRequestHandler<UpdateLawCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public UpdateLawCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(UpdateLawCommand request, CancellationToken cancellationToken)
        {
            var law = await _context.Law.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (law == null)
                return CommandResponse.Failure(400, "قانون انتخاب شده در سیستم وجود ندارد");

            law.ApprovalDate = request.ApprovalDate;
            law.ApprovalStatusId = request.ApprovalStatusId;
            law.ApprovalAuthorityId = request.ApprovalAuthorityId;
            law.ApprovalTypeId = request.ApprovalTypeId;
            law.ExecutorManagmentId = request.ExecutorManagmentId;
            law.LawCategoryId = request.LawCategoryId;
            law.Title = request.Title;
            law.Description = request.Description;
            law.IsOriginal = request.IsOriginal;
            law.Announcement = new Announcement(request.AnnouncementNumber, request.AnnouncementDate);
            law.Newspaper = new Newspaper(request.NewspaperNumber, request.NewspaperDate);
            law.Type = request.Type == 0 ? LawType.Rule : LawType.Regulation;

            _context.Law.Update(law);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
