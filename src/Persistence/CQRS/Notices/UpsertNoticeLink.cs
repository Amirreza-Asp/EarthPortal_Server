using Application.CQRS.Notices;
using Application.Models;
using Domain.Entities.Notices;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Notices
{
    public class UpsertNoticeLinkCommandHandler : IRequestHandler<UpsertNoticeLinkCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public UpsertNoticeLinkCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(UpsertNoticeLinkCommand request, CancellationToken cancellationToken)
        {
            // حذف کلید واژه هایی که به هیچ خبری مرتبط نیستند
            var emptyLinks = await _context.Link.Where(b => b.NoticeLinks.Count == 0).AsNoTracking().ToListAsync(cancellationToken);
            _context.Link.RemoveRange(emptyLinks);

            var oldRelationLinks = await _context.NoticeLink.Where(b => b.NoticeId == request.NoticeId).AsNoTracking().ToListAsync(cancellationToken);
            _context.NoticeLink.RemoveRange(oldRelationLinks);

            var availableLinks = await _context.Link.Where(b => request.Links.Contains(b.Value)).AsNoTracking().ToListAsync(cancellationToken);
            var unavailableLinks = request.Links.Where(link => !availableLinks.Select(b => b.Value).Contains(link)).Select(b => new Link(Guid.NewGuid(), b));

            if (unavailableLinks.Any())
            {
                // افزودن کلید واژه های جدید
                _context.Link.AddRange(unavailableLinks);
                await _context.SaveChangesAsync();
            }

            var addedLinks = await _context.Link.Where(b => unavailableLinks.Select(e => e.Value).Contains(b.Value)).AsNoTracking().ToListAsync();

            var noticeLinks = availableLinks.Select(b => new NoticeLink(request.NoticeId, b.Id)).ToList();
            noticeLinks.AddRange(addedLinks.Select(b => new NoticeLink(request.NoticeId, b.Id)).ToList());

            _context.NoticeLink.AddRange(noticeLinks);

            await _context.SaveChangesAsync();
            return CommandResponse.Success();
        }
    }
}
