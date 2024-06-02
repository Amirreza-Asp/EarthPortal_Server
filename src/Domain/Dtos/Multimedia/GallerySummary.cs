using Domain.Dtos.Shared;

namespace Domain.Dtos.Multimedia
{
    public class GallerySummary
    {
        public Guid Id { get; set; }
        public String Title { get; set; }
        public String Description { get; set; }
        public List<ImageSummary> Images { get; set; }
        public int Order { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
