using Application.CQRS.Regulation.ExecutorManagements;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Persistence.CQRS.Regulation.ExecutorManagements
{
    public class UpdateExecutorManagementCommandHandler : IRequestHandler<UpdateExecutorManagementCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public UpdateExecutorManagementCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(UpdateExecutorManagementCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.ExecutorManagment.FirstOrDefaultAsync(b => b.Id == request.Id);

            if (entity == null)
                return CommandResponse.Failure(400, "دستگاه مجری انتخاب شده در سیستم وجود ندارد");

            entity.Name = request.Title;

            _context.ExecutorManagment.Update(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
