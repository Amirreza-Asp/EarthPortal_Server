using Domain.Shared;

namespace Domain.Entities.Regulation
{
    //دستگاه مجری
    public class ExecutorManagment : BaseEntity
    {
        public ExecutorManagment(string name)
        {
            Name = name;
        }

        public String Name { get; set; }
    }
}
