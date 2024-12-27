namespace TestTask.Services;

using Data.Models;
using Data.Repositories;

public class ReportService(IReportRepository reportRepository)
{
    public async Task<IEnumerable<Report>> GenerateReport(int itemsPerYear)
    {
        return await reportRepository.GetTopNEachYear(itemsPerYear);
    }
}
