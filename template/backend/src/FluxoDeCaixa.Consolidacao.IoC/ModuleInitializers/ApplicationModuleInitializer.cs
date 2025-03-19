using FluxoDeCaixa.Consolidacao.Application.Interfaces;
using FluxoDeCaixa.Consolidacao.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FluxoDeCaixa.Consolidacao.IoC.ModuleInitializers;
public class ApplicationModuleInitializer : IModuleInitializer
{
    public void Initialize(HostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<ICashRegisterAppService, CashRegisterAppService>();
    }
}