﻿using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Resources.Broadcasts;
using Application.Models;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Resources.Broadcasts
{
    public class RemoveBroadcastCommandHandler : IRequestHandler<RemoveBroadcastCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _env;
        private readonly ILogger<RemoveBroadcastCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public RemoveBroadcastCommandHandler(ApplicationDbContext context, IHostingEnvironment env, ILogger<RemoveBroadcastCommandHandler> logger, IUserAccessor userAccessor)
        {
            _context = context;
            _env = env;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(RemoveBroadcastCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Broadcast.FirstOrDefaultAsync(b => b.Id == request.Id);
            var upload = _env.WebRootPath;

            if (entity == null)
                return CommandResponse.Failure(400, "نشریه مورد نظر در سیستم وجود ندارد");

            if (File.Exists(upload + SD.BroadcastFilePath + entity.File))
                File.Delete(upload + SD.BroadcastFilePath + entity.File);

            if (File.Exists(upload + SD.BroadcastImagePath + entity.Image))
                File.Delete(upload + SD.BroadcastImagePath + entity.Image);

            _context.Broadcast.Remove(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _logger.LogInformation($"Broadcast with id {entity.Id} removed by {_userAccessor.GetUserName()} in {DateTime.Now}");
                return CommandResponse.Success();
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
