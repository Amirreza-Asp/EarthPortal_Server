using Domain.Shared;

namespace Domain.Entities.Regulation
{
    //نوع مصوبه
    public class ApprovalType : BaseEntity
    {
        public ApprovalType(string value) : base(Guid.NewGuid())
        {
            Value = value;
        }

        private ApprovalType()
        {

        }

        public String Value { get; set; }
    }
}
