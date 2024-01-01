using Domain.Shared;

namespace Domain.Entities.Regulation
{
    //مرجع تصویب
    public class ApprovalAuthority : BaseEntity
    {
        public ApprovalAuthority(string name)
        {
            Name = name;
        }

        public String Name { get; set; }
    }
}
