using AutoMapper;
using CSharpFunctionalExtensions;
using FluxoDeCaixa.Consolidacao.Application.Interfaces;
using FluxoDeCaixa.Consolidacao.Application.RequestResponse;
using FluxoDeCaixa.Consolidacao.Common.Enums;
using FluxoDeCaixa.Consolidacao.Common.Utils;
using FluxoDeCaixa.Consolidacao.Domain.Services;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text.Json;

namespace FluxoDeCaixa.Consolidacao.Application.Services;

public class CashRegisterAppService : BaseService<CashRegisterAppService>, ICashRegisterAppService
{
    private readonly IS3Service _s3;
    public CashRegisterAppService(
        IS3Service s3,
        IMapper mapper,
        ILogger<CashRegisterAppService> logger) : base(mapper, logger)
    {
        _s3 = s3;
    }

    public async Task<Result<bool>> CalculateBalance(string messageDataJson, CancellationToken cancellationToken = default)
    {
        try
        {
            // verificar se existe o arquivo
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            CashRegisterRequest messageData = JsonConvert.DeserializeObject<CashRegisterRequest>(messageDataJson, settings);
            var nameBalance = $"{messageData?.StoreId.ToString()}/balance.json";

            bool balanceExists = await _s3.FileExistsAsync(nameBalance);
            CashRegister currentBalance = default;
            if (balanceExists)
            {
                currentBalance = await DeserializeObjectBalance(nameBalance);
                currentBalance.CurrentBalance = messageData?.CashRegisterType switch
                {
                    CashRegisterType.Credit => currentBalance?.CurrentBalance + messageData.Subtotal,
                    CashRegisterType.Debit => currentBalance?.CurrentBalance - messageData.Subtotal,
                    _ => currentBalance?.CurrentBalance
                };
            }
            else
                currentBalance = new CashRegister { CurrentBalance = messageData?.Subtotal };

            string strUpdateBalance = JsonConvert.SerializeObject(currentBalance, Formatting.Indented);
            await _s3.SaveFileAsync(nameBalance, strUpdateBalance.ToStream(), "application/json");

            return Result.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro no processamento para caixa registradora regra de negocio, mensagria");
            return Result.Failure<bool>(ex.Message);
        }
    }

    private async Task<CashRegister> DeserializeObjectBalance(string nameBalance)
    {
        using var s3ResultGetFileStream = await _s3.GetFileAsync(nameBalance);
        using var reader = new StreamReader(s3ResultGetFileStream);
        var str = await reader.ReadToEndAsync();
        var balanceData = JsonConvert.DeserializeObject<CashRegister>(str);
        return balanceData ?? new CashRegister { CurrentBalance = 0 };
    }
}
