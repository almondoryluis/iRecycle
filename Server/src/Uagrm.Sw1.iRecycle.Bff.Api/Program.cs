using FastEndpoints.Swagger;
using FastEndpoints;
using HealthChecks.UI.Client;
using Serilog.Events;
using Serilog;
using System.Text.Json.Serialization;
using Destructurama;
using Uagrm.Sw1.iRecycle.Bff.Core;
using Uagrm.Sw1.iRecycle.Bff.Infrastructure;
using Uagrm.Sw1.iRecycle.Bff.Api;
using Uagrm.Sw1.iRecycle.Bff.Api.Middlewares;

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Host.UseSerilog();

    builder.Services.AddFastEndpoints();
    builder.Services.SwaggerDocument(o =>
    {
        o.ShortSchemaNames = true;
    });

    builder.Services.ConfigureHttpJsonOptions(jsonOptions =>
    {
        jsonOptions.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

    builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(jsonOptions =>
    {
        jsonOptions.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });


    builder.Services.AddCoreServices();
    builder.Services.AddInfrastructureServices(options =>
    {
        options.ConnectionStrings = builder.Configuration.GetSection("ConnectionStrings").Get<ConnectionStrings>();
    });

    builder.Services.AddApiServices();


    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowSpecificOrigin",
            builder => builder
            //.WithOrigins(corsUri!)
            //.AllowCredentials()
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod()
            .WithHeaders(["accept", "content-type", "origin", "x-requested-with", "Access-Control-Allow-Origin"])
            );
    });

    var app = builder.Build();

    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(app.Configuration)
        .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .Destructure.UsingAttributes()
        .CreateLogger();


    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseCors("AllowSpecificOrigin");
    app.UseCors(x => x.AllowAnyMethod());

    app.UseFastEndpoints(c =>
    {
        c.Errors.UseProblemDetails();

    });

    //app.UseAuthentication();
    //app.UseAuthorization();

    app.UseSwaggerGen();

    //Exception Middleware
    app.UseMiddleware<CustomExceptionHandlerMiddleware>();



    app.UseHealthChecks("/health",
        new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.Fatal("Application terminated unexpectedly");
    Log.CloseAndFlush();
}

public partial class Program { }