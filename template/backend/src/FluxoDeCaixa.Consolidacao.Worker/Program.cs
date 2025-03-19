using FluxoDeCaixa.Consolidacao.IoC;
using FluxoDeCaixa.Consolidacao.Worker;
using Serilog;
var builder = Host.CreateApplicationBuilder(args);
builder.InitializerModulesNow();
builder.Services.AddHostedService<SqsConsumerWorker>();
var host = builder.Build();
try
{
    Log.Information("Iniciando SQS Consumer Worker");
    await host.RunAsync();
    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "SQS Consumer Worker terminou inesperadamente");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}