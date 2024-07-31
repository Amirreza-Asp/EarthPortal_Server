using Domain.Shared;

namespace Domain.Entities.Contact
{
    public class EducationalVideo : BaseEntity
    {
        public EducationalVideo(string title, string description, string video) : base(Guid.NewGuid())
        {
            Title = title;
            Description = description;
            Video = video;
        }


        private EducationalVideo()
        {
        }

        public string Title { get; set; }
        public string Description { get; set; }
        public string Video { get; set; }
    }
}
