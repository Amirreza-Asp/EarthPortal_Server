using Domain.Shared;

namespace Domain.Entities.Mutimedia
{
    public class Infographic : BaseEntity
    {
        public Infographic(string name, string title)
        {
            Name = name;
            Title = title;
        }

        public String Title { get; set; }
        public String Name { get; set; }
        public bool IsLandscape { get; set; }
    }
}
