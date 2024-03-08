using Domain.Shared;

namespace Domain.Dtos.Contact
{
    public class RelatedCompany : BaseEntity
    {
        public RelatedCompany(string name, string image, int order)
        {
            Name = name;
            Image = image;
            Order = order;
        }

        public String Name { get; set; }
        public String Image { get; set; }
        public int Order { get; set; }
    }
}
