using System.Reflection;
using Uagrm.Sw1.iRecycle.Bff.Api.Middlewares;

namespace Uagrm.Sw1.iRecycle.Bff.Api;

public static class ConfigureApiServices
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddHealthChecks();
        services.AddScoped<CustomExceptionHandlerMiddleware>();
        services.AddSignalR();
        return services;
    }
}
