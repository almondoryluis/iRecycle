using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Uagrm.Sw1.iRecycle.Bff.Infrastructure.Database;

namespace Uagrm.Sw1.iRecycle.Bff.Infrastructure;

public static class StartupSetup
{
    public static void AddDbContext(this IServiceCollection services, string connectionString) =>
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString,
                sqlServerOptions =>
                {
                    sqlServerOptions.MigrationsAssembly(typeof(ConfigureServices).Namespace);
                    sqlServerOptions.EnableRetryOnFailure();
                }));
}
