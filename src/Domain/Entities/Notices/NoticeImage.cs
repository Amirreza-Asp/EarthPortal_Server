using Domain.Shared;

namespace Domain.Entities.Notices
{
    public class NoticeImage : BaseEntity
    {
        public NoticeImage(string name, Guid noticeId, int order) : base(Guid.NewGuid())
        {
            Name = name;
            NoticeId = noticeId;
            Order = order;
        }

        private NoticeImage() { }

        public String Name { get; set; }
        public Guid NoticeId { get; set; }
        public int Order { get; set; }

        public Notice? Notices { get; set; }
    }
}
