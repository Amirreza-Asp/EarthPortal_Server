using Domain.Shared;

namespace Domain.Entities.Contact
{
    public class SystemEvaluationPage : BaseEntity
    {
        public SystemEvaluationPage(Enums.Pages page, Guid systemEvaluationId)
        {
            Page = page;
            SystemEvaluationId = systemEvaluationId;
        }

        public Enums.Pages Page { get; set; }
        public Guid SystemEvaluationId { get; set; }

        public SystemEvaluation SystemEvaluation { get; set; }
    }
}
