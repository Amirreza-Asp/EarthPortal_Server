using Domain.Shared;

namespace Domain.Entities.Notices
{
    public class Notice : BaseEntity
    {
        public Notice(string title, string description, string headline, string source, DateTime dateOfRegisration, Guid noticeCategoryId, int shortLink, bool isActive) : base(Guid.NewGuid())
        {
            Title = title;
            Description = description;
            Headline = headline;
            Source = source;
            DateOfRegisration = dateOfRegisration;
            NoticeCategoryId = noticeCategoryId;
            ShortLink = shortLink;
            IsActive = isActive;
        }

        private Notice() { }

        public String Title { get; set; }
        public String Description { get; set; }
        public String Headline { get; set; }
        public String Source { get; set; }
        public DateTime DateOfRegisration { get; set; }
        public int ShortLink { get; set; }
        public long Seen { get; set; }
        public bool IsActive { get; set; } = true;

        public Guid NoticeCategoryId { get; set; }

        public NoticeCategory? NoticeCategory { get; set; }
        public ICollection<NoticeLink> Links { get; set; }
        public ICollection<NoticeImage> Images { get; set; }
    }
}
