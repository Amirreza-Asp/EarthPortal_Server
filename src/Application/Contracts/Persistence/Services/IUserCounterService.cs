namespace Application.Contracts.Persistence.Services
{
    public interface IUserCounterService
    {
        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
