using Domain.Shared;

namespace Domain.Entities.Mutimedia
{
    public class VideoContent : BaseEntity
    {
        public VideoContent(string title, string description, string video)
        {
            Title = title;
            Description = description;
            Video = video;
        }

        private VideoContent() { }

        public String Title { get; set; }
        public String Description { get; set; }
        public String Video { get; set; }
    }
}
