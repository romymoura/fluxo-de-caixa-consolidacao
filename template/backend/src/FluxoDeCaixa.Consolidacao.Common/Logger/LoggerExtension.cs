using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace FluxoDeCaixa.Consolidacao.Common.Logger;

public static class LoggerExtension
{
    public static HostApplicationBuilder AddDefaultLogger(this HostApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File("logs/sqs-consumer-.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();
        builder.Services.AddSerilog();
        builder.Logging.AddSerilog(dispose: true);
        return builder;
    }
}