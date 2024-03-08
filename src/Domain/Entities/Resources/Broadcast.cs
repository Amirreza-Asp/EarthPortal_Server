using Domain.Shared;

namespace Domain.Entities.Resources
{
    public class Broadcast : BaseEntity
    {
        public Broadcast(string title, Guid authorId, string image)
        {
            Title = title;
            AuthorId = authorId;
            Image = image;
        }

        private Broadcast() { }
        
        public String Title { get; set; }
        public String Image { get; set; }
        public Guid AuthorId { get; set; }

        public Author? Author { get; set; }
    }
}
