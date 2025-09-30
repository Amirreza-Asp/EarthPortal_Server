using Domain.Shared;

namespace Domain.Entities.Notices
{
    public class NoticeCategory : BaseEntity
    {
        public NoticeCategory(string title, string? description) : base(Guid.NewGuid())
        {
            Title = title;
            Description = description;
        }

        private NoticeCategory() { }

        public String Title { get; set; }
        public String? Description { get; set; }
    }
}
