using Domain.Shared;

namespace Domain.Entities.Contact
{
    public class AboutUs : BaseEntity
    {
        public AboutUs(
            string title,
            string content,
            string? video,
            string? image,
            DateTime accomplishedDate
        )
            : base(Guid.NewGuid())
        {
            Title = title;
            Content = content;
            Video = video;
            Image = image;
            AccomplishedDate = accomplishedDate;
        }

        private AboutUs() { }

        public String Title { get; set; }
        public String Content { get; set; }
        public String? Video { get; set; }
        public String? Image { get; set; }
        public DateTime AccomplishedDate { get; set; }
    }
}
