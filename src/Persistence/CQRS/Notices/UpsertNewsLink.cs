﻿using Application.CQRS.Notices;
using Application.Models;
using Domain.Entities.Notices;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Notices
{
    public class UpsertNewsLinkCommandHandler : IRequestHandler<UpsertNewsLinkCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public UpsertNewsLinkCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(UpsertNewsLinkCommand request, CancellationToken cancellationToken)
        {
            // حذف کلید واژه هایی که به هیچ خبری مرتبط نیستند
            var emptyLinks = await _context.Link.Where(b => b.NewsLinks.Count == 0).AsNoTracking().ToListAsync(cancellationToken);
            _context.Link.RemoveRange(emptyLinks);

            var oldRelationLinks = await _context.NewsLink.Where(b => b.NewsId == request.NewsId).AsNoTracking().ToListAsync(cancellationToken);
            _context.NewsLink.RemoveRange(oldRelationLinks);

            var availableLinks = await _context.Link.Where(b => request.Links.Contains(b.Value)).AsNoTracking().ToListAsync(cancellationToken);
            var unavailableLinks = request.Links.Where(link => !availableLinks.Select(b => b.Value).Contains(link)).Select(b => new Link(Guid.NewGuid(), b));

            if (unavailableLinks.Any())
            {
                // افزودن کلید واژه های جدید
                _context.Link.AddRange(unavailableLinks);
                await _context.SaveChangesAsync();
            }

            var addedLinks = await _context.Link.Where(b => unavailableLinks.Select(e => e.Value).Contains(b.Value)).AsNoTracking().ToListAsync();

            var newLinks = availableLinks.Select(b => new NewsLink(request.NewsId, b.Id)).ToList();
            newLinks.AddRange(addedLinks.Select(b => new NewsLink(request.NewsId, b.Id)).ToList());

            _context.NewsLink.AddRange(newLinks);

            await _context.SaveChangesAsync();
            return CommandResponse.Success();
        }
    }
}
