namespace FluxoDeCaixa.Consolidacao.Common.RequestResponse;

public class ResponseRequestWithData<T> : ResponseRequest
{
    public T? Data { get; set; }
}
