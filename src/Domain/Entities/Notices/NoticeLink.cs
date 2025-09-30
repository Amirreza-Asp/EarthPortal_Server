namespace Domain.Entities.Notices
{
    public class NoticeLink
    {
        public NoticeLink(Guid noticeId, Guid linkId)
        {
            NoticeId = noticeId;
            LinkId = linkId;
        }

        private NoticeLink() { }

        public Guid NoticeId { get; set; }
        public Guid LinkId { get; set; }

        public Notice? Notices { get; set; }
        public Link? Link { get; set; }
    }
}
