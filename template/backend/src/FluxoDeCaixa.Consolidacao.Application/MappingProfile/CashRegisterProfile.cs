using AutoMapper;

namespace FluxoDeCaixa.Consolidacao.Application.MappingProfile;

public class CashRegisterProfile : Profile
{
    public CashRegisterProfile()
    {
        // Response da Cloud para Application, para retorno do contrato.
        CreateMap<Amazon.SQS.Model.SendMessageResponse, RequestResponse.CashRegisterResponse>()
            .BeforeMap((source, destination) =>
            {
                destination.MessageId = new Guid(source.MessageId);
            });
    }
}