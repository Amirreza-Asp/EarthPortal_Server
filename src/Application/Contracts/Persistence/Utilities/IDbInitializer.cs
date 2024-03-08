namespace Application.Contracts.Persistence.Utilities
{
    public interface IDbInitializer
    {
        Task Execute();
    }
}
