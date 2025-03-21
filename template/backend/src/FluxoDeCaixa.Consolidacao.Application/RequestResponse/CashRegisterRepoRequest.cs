﻿using FluxoDeCaixa.Consolidacao.Common.Enums;

namespace FluxoDeCaixa.Consolidacao.Application.RequestResponse;

public class CashRegisterRepoRequest
{
    public string? ProductId { get; set; }
    public string? StoreId { get; set; }
    public string? MessageId { get; set; }
    public DateTime? CreateDate { get; set; }
    public decimal? Price { get; set; } = 0;
    public int? Amount { get; set; } = 1;
    public decimal? Subtotal
    {
        get
        {
            return Price * (CashRegisterType == CashRegisterType.Credit ? Amount : 1);
        }
    }
    public CashRegisterType CashRegisterType { get; set; }
}
