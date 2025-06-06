﻿using Application.CQRS.Notices;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Notices
{
    public class RemoveNewsCategoryCommandHandler : IRequestHandler<RemoveNewsCategoryCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public RemoveNewsCategoryCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(RemoveNewsCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _context.NewsCategory.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (category == null)
                return CommandResponse.Failure(400, "آیتم مورد نظر در سیستم وجود ندارد");

            _context.Remove(category);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
