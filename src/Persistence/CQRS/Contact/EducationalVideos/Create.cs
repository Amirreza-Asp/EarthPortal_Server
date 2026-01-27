using Application.Contracts.Infrastructure.Services;
using Application.CQRS.Contact.EducationalVideos;
using Application.Models;
using Domain.Entities.Contact;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Persistence.CQRS.Contact.EducationalVideos
{
    public class CreateEducationalVideoCommandHandler
        : IRequestHandler<CreateEducationalVideoCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CreateEducationalVideoCommandHandler> _logger;
        private readonly IUserAccessor _userAccessor;

        public CreateEducationalVideoCommandHandler(
            ApplicationDbContext context,
            ILogger<CreateEducationalVideoCommandHandler> logger,
            IUserAccessor userAccessor
        )
        {
            _context = context;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(
            CreateEducationalVideoCommand request,
            CancellationToken cancellationToken
        )
        {
            if (!request.Video.Contains("</iframe>"))
                return CommandResponse.Failure(400, "فرمت ویدیو وارد شده صحیح نیست");

            var edv = new EducationalVideo(request.Title, request.Description, request.Video);
            edv.Order = request.Order;

            _context.EducationalVideo.Add(edv);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                _logger.LogInformation(
                    "EducationalVideo with id {Username} registered by {UserRealName} in {DoneTime}",
                    edv.Id,
                    _userAccessor.GetUserName(),
                    DateTimeOffset.UtcNow
                );

                return CommandResponse.Success(edv.Id);
            }

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
