using CSharpFunctionalExtensions;

namespace FluxoDeCaixa.Consolidacao.Application.Interfaces;

public interface ICashRegisterAppService
{
    Task<Result<bool>> CalculateBalance(string messageData, CancellationToken cancellationToken = default);
}
