using Domain.Shared;

namespace Domain.Entities.Contact
{
    public class AboutUs : BaseEntity
    {
        public AboutUs(string title, string content, string? video, string? image) : base(Guid.NewGuid())
        {
            Title = title;
            Content = content;
            Video = video;
            Image = image;
        }

        private AboutUs() { }

        public String Title { get; set; }
        public String Content { get; set; }
        public String? Video { get; set; }
        public String? Image { get; set; }
    }
}
