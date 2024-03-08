using Domain.Entities.Contact.Enums;
using Domain.Shared;

namespace Domain.Entities.Contact
{
    public class SystemEvaluationPage : BaseEntity
    {
        public SystemEvaluationPage(Pages page, Guid systemEvaluationId)
        {
            Page = page;
            SystemEvaluationId = systemEvaluationId;
        }

        public Pages Page { get; set; }
        public Guid SystemEvaluationId { get; set; }

        public SystemEvaluation SystemEvaluation { get; set; }
    }
}
