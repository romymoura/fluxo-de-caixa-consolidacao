using Microsoft.Extensions.Hosting;

namespace FluxoDeCaixa.Consolidacao.IoC;
public interface IModuleInitializer
{
    void Initialize(HostApplicationBuilder builder);
}
