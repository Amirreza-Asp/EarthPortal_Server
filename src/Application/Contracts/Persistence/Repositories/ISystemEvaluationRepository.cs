using Domain.Dtos.Contact;

namespace Application.Contracts.Persistence.Repositories
{
    public interface ISystemEvaluationRepository
    {
        Task<SystemEvaluationDetails> GetAsync(CancellationToken cancellationToken);
    }
}
