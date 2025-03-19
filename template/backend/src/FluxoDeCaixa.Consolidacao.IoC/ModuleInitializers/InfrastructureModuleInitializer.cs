using Microsoft.Extensions.Hosting;

namespace FluxoDeCaixa.Consolidacao.IoC.ModuleInitializers;

public class InfrastructureModuleInitializer : IModuleInitializer
{
    public void Initialize(HostApplicationBuilder builder)
    {
        RegisterDependencies(ref builder);
    }
    private void RegisterDependencies(ref HostApplicationBuilder builder)
    {
    }
}
