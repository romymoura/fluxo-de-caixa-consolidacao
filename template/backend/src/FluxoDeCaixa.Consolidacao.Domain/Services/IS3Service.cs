namespace FluxoDeCaixa.Consolidacao.Domain.Services;

public interface IS3Service
{
    Task<Stream> GetFileAsync(string key);
    Task<bool> FileExistsAsync(string key);
    Task SaveFileAsync(string key, Stream content, string contentType = null);
}
