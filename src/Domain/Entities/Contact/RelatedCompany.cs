using Domain.Shared;

namespace Domain.Entities.Contact
{
    public class RelatedCompany : BaseEntity
    {
        public RelatedCompany(string name, string image, int order)
        {
            Name = name;
            Image = image;
            Order = order;
        }

        public string Name { get; set; }
        public string Image { get; set; }
        public int Order { get; set; }
        public string Link { get; set; }
    }
}
