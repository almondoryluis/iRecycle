using Microsoft.Extensions.DependencyInjection;
using Uagrm.Sw1.iRecycle.Bff.Infrastructure.Database;
using Uagrm.Sw1.iRecycle.Bff.Infrastructure.Database.Exceptions;
using Uagrm.Sw1.iRecycle.Bff.SharedKernel.Interfaces;

namespace Uagrm.Sw1.iRecycle.Bff.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, Action<ConfigureServicesRegistration> options)
    {
        ConfigureServicesRegistration configureServicesRegistration = new();
        options.Invoke(configureServicesRegistration);

        services.AddDatabase(configureServicesRegistration);

        return services;
    }



    private static IServiceCollection AddDatabase(this IServiceCollection services, ConfigureServicesRegistration configureServicesRegistration)
    {
        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        if (string.IsNullOrEmpty(configureServicesRegistration.ConnectionStrings?.RelationalDbConnection))
        {
            throw new InvalidConnectionStringException("Invalid default connection-string on Infrastructure services");
        }
        else
        {
            services.AddDbContext(configureServicesRegistration.ConnectionStrings!.RelationalDbConnection);
        }

        return services;
    }

}



public class ConfigureServicesRegistration
{
    public ConnectionStrings? ConnectionStrings { get; set; }

}


public class ConnectionStrings
{
    public string? RelationalDbConnection { get; set; }
}