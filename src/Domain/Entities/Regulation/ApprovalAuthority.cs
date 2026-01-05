using Domain.Shared;

namespace Domain.Entities.Regulation
{
    //مرجع تصویب
    public class ApprovalAuthority : BaseEntity
    {
        public ApprovalAuthority(string name)
            : base(Guid.NewGuid())
        {
            Name = name;
        }

        private ApprovalAuthority() { }

        public String Name { get; set; }
    }
}
