using Application.CQRS.Regulation.ExecutorManagements;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Regulation.ExecutorManagements
{
    public class RemoveExecutorManagementCommandHandler : IRequestHandler<RemoveExecutorManagementCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public RemoveExecutorManagementCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(RemoveExecutorManagementCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.ExecutorManagment.FirstOrDefaultAsync(b => b.Id == request.Id);

            if (entity == null)
                return CommandResponse.Failure(400, "دستگاه مجری انتخاب شده در سیستم وجود ندارد");


            _context.ExecutorManagment.Remove(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
