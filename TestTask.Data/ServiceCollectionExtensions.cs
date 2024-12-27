using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestTask.Utils;

namespace TestTask.Data;

public static class ServiceCollectionExtensions
{
    public static void AddTestDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.RegisterConfig<DbConfig>(configuration, nameof(DbConfig));
        services.AddDbContextFactory<TestDbContext>((s, n) =>
            n.UseNpgsql(s.GetRequiredService<DbConfig>().ConnectionString, opts => opts.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), null))
        );
        // services.AddDbContext<TestDbContext>((s, n) =>
        //     n.UseNpgsql(s.GetRequiredService<DbConfig>().ConnectionString));
    }
}
