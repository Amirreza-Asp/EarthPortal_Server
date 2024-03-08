using Domain.Shared;

namespace Domain.Entities.Mutimedia
{
    public class Gallery : BaseEntity
    {
        public Gallery(string title, string description)
        {
            Title = title;
            Description = description;
        }

        private Gallery() { }

        public void AddImages(List<GalleryPhoto> images)
        {
            images.ForEach(image => Images.Add(image));
        }

        public String Title { get; set; }
        public String Description { get; set; }
        public ICollection<GalleryPhoto> Images { get; set; } = new List<GalleryPhoto>();
    }
}
