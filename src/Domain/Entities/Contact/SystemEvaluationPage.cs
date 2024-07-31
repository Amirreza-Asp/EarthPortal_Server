using Domain.Shared;

namespace Domain.Entities.Contact
{
    public class SystemEvaluationPage : BaseEntity
    {
        public SystemEvaluationPage(Enums.Pages page, Guid systemEvaluationId) : base(Guid.NewGuid())
        {
            Page = page;
            SystemEvaluationId = systemEvaluationId;
        }

        private SystemEvaluationPage() { }

        public Enums.Pages Page { get; set; }
        public Guid SystemEvaluationId { get; set; }

        public SystemEvaluation SystemEvaluation { get; set; }
    }
}
