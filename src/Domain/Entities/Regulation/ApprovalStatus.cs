using Domain.Shared;

namespace Domain.Entities.Regulation
{
    public class ApprovalStatus : BaseEntity
    {
        public ApprovalStatus(string status) : base(Guid.NewGuid())
        {
            Status = status;
        }

        private ApprovalStatus() { }

        public String Status { get; set; }
    }
}
