using Domain.Shared;

namespace Domain.Entities.Mutimedia
{
    public class GalleryPhoto : BaseEntity
    {
        public GalleryPhoto(string name, int order, Guid galleryId)
        {
            Name = name;
            Order = order;
            GalleryId = galleryId;
        }

        private GalleryPhoto() { }


        public string Name { get; set; }
        public Guid GalleryId { get; set; }

        public Gallery? Gallery { get; set; }

    }
}
