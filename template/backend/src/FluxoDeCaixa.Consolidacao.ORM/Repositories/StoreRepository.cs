using FluxoDeCaixa.Consolidacao.Domain.Entities;
using FluxoDeCaixa.Consolidacao.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace FluxoDeCaixa.Consolidacao.ORM.Repositories;

public class StoreRepository : BaseRepository<StoreRepository, Store>, IStoreRepository
{
    public StoreRepository(DefaultContext context, ILogger<StoreRepository> logger) : base(context, logger) { }
}
