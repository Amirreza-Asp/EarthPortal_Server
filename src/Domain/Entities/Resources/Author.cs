using Domain.Shared;

namespace Domain.Entities.Resources
{
    public class Author : BaseEntity
    {
        public Author(string name, string family, string? country)
        {
            Name = name;
            Family = family;
            Country = country;
        }

        private Author() { }

        public String Name { get; set; }
        public String Family { get; set; }
        public String? Country { get; set; }
    }
}
