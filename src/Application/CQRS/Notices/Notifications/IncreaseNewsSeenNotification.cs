using Application.Contracts.Persistence.Repositories;
using MediatR;

namespace Application.CQRS.Notices.Notifications
{
    public class IncreaseNewsSeenNotification : INotification
    {
        public IncreaseNewsSeenNotification(int shortLink)
        {
            ShortLink = shortLink;
        }

        public int ShortLink { get; set; }
    }

    public class IncreaseNewsSeenNotificationHandler : INotificationHandler<IncreaseNewsSeenNotification>
    {
        private readonly INewsRepository _newsRepository;

        public IncreaseNewsSeenNotificationHandler(INewsRepository newsRepository)
        {
            _newsRepository = newsRepository;
        }

        public async Task Handle(IncreaseNewsSeenNotification notification, CancellationToken cancellationToken)
        {
            var news =
                await _newsRepository
                    .FirstOrDefaultAsync(
                        filter: b => b.ShortLink == notification.ShortLink,
                        cancellationToken: cancellationToken);

            if (news == null)
                return;

            news.Seen = news.Seen + 1;

            _newsRepository.Update(news);
            await _newsRepository.SaveAsync(cancellationToken);
        }
    }
}
