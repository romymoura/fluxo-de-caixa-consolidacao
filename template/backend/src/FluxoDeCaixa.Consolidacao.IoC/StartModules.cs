using FluxoDeCaixa.Consolidacao.Application;
using FluxoDeCaixa.Consolidacao.IoC.ModuleInitializers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using FluxoDeCaixa.Consolidacao.Common.Logger;


namespace FluxoDeCaixa.Consolidacao.IoC;
public static class StartModules
{
    public static void InitializerModulesNow(this HostApplicationBuilder builder)
    {
        builder.InitLogger();
        builder.InitAutoMapper();
        new ApplicationModuleInitializer().Initialize(builder);
        new InfrastructureModuleInitializer().Initialize(builder);
        new CloudModuleInitializer().Initialize(builder);
    }
    private static void InitLogger(this HostApplicationBuilder builder)
    {
        builder.AddDefaultLogger();
    }
    private static void InitAutoMapper(this HostApplicationBuilder builder)
    {
        Assembly apiAssembly = Assembly.Load("FluxoDeCaixa.Consolidacao.Worker");
        builder.Services.AddAutoMapper(apiAssembly, typeof(ApplicationLayer).Assembly);
    }
}
