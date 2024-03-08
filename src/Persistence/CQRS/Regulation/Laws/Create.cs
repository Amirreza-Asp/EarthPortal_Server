using Application.CQRS.Regulation.Laws;
using Application.Models;
using Domain.Entities.Regulation;
using Domain.Entities.Regulation.Enums;
using Domain.Entities.Regulation.ValueObjects;
using MediatR;

namespace Persistence.CQRS.Regulation.Laws
{
    public class CreateLawCommandHandler : IRequestHandler<CreateLawCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public CreateLawCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(CreateLawCommand request, CancellationToken cancellationToken)
        {
            var law = new Law(request.Title, new Announcement(request.AnnouncementNumber, request.AnnouncementDate), new Newspaper(request.AnnouncementNumber, request.NewspaperDate),
                             request.Description, request.ApprovalDate, request.Type == 0 ? LawType.Rule : LawType.Regulation, request.IsOriginal, request.ApprovalTypeId, request.ApprovalStatusId,
                             request.ExecutorManagmentId, request.ApprovalAuthorityId, request.LawCategoryId);

            _context.Law.Add(law);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success(law.Id);

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
