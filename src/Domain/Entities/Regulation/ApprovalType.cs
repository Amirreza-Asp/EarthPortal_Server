using Domain.Shared;

namespace Domain.Entities.Regulation
{
    //نوع مصوبه
    public class ApprovalType : BaseEntity
    {
        public ApprovalType(string value)
        {
            Value = value;
        }

        public String Value { get; set; }
    }
}
