using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace App.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddDependencies(this IServiceCollection services, HostBuilderContext context)
    {
        services.AddTransient<IWorker, Worker>();
        services.AddSingleton(GetTelemetryClient(context));
    }

    private static TelemetryClient GetTelemetryClient(HostBuilderContext context)
    {
        var connectionString = context.Configuration["Serilog:WriteTo:1:Args:connectionString"];
        var telemetryConfiguration = TelemetryConfiguration.CreateDefault();
        telemetryConfiguration.ConnectionString = connectionString;
        var telemetryClient = new TelemetryClient(telemetryConfiguration);
        return telemetryClient;
    }
}