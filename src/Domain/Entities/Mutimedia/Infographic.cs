using Domain.Shared;

namespace Domain.Entities.Mutimedia
{
    public class Infographic : BaseEntity
    {
        public Infographic(string name)
        {
            Name = name;
        }

        public String Name { get; set; }
    }
}
