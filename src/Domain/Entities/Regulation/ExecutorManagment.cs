using Domain.Shared;

namespace Domain.Entities.Regulation
{
    //دستگاه مجری
    public class ExecutorManagment : BaseEntity
    {
        public ExecutorManagment(string name)
            : base(Guid.NewGuid())
        {
            Name = name;
        }

        private ExecutorManagment() { }

        public String Name { get; set; }
    }
}
