namespace TestTask.Data.Repositories;

using Models;

public interface IReportRepository
{
    Task<IEnumerable<Report>> GetTopNEachYear(int n);
}
