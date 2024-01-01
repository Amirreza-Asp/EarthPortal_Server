using Domain.Shared;

namespace Domain.Entities.Regulation
{
    public class ApprovalStatus : BaseEntity
    {
        public ApprovalStatus(string status)
        {
            Status = status;
        }

        public String Status { get; set; }
    }
}
