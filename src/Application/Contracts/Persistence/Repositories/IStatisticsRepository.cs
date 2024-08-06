namespace Application.Contracts.Persistence.Repositories
{
    public interface IStatisticsRepository
    {
        Task<int> GetTodaySeen();
        Task<long> GetTotalSeen();
        Task<int> GetMonthSeen();
        Task<long> GetYearSeen();
    }
}
