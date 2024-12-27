using Microsoft.Extensions.DependencyInjection;

namespace TestTask.Services;

using Data.Repositories;

public static class ServiceCollectionExtensions
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<MarketService>();
        services.AddScoped<ReportService>();
        services.AddScoped<IReportRepository, ReportRepositoryDbContext>();
    }
}