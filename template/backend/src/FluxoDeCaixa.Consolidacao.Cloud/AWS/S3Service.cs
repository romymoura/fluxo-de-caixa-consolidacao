using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using FluxoDeCaixa.Consolidacao.Common.Configuration;
using FluxoDeCaixa.Consolidacao.Domain.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace FluxoDeCaixa.Consolidacao.Cloud.AWS;


public class S3Service : IS3Service
{
    private readonly ILogger<S3Service> _logger;
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;
    

    public S3Service(ILogger<S3Service> logger, IOptions<S3Config> config, IAmazonS3 s3Client)
    {
        _logger = logger;
        _bucketName = config.Value.BucketName;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _s3Client = s3Client;
    }

    public async Task<Stream> GetFileAsync(string key)
    {
        try
        {
            _logger.LogInformation("Buscando arquivo do S3: {BucketName}/{Key}", _bucketName, key);
            var request = new GetObjectRequest
            {
                BucketName = _bucketName,
                Key = key
            };

            var response = await _s3Client.GetObjectAsync(request);

            _logger.LogInformation("Arquivo encontrado: {Key}, tamanho: {Size} bytes",
                key, response.ContentLength);

            // Criando um MemoryStream para armazenar o conteúdo do arquivo
            var memoryStream = new MemoryStream();
            await response.ResponseStream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            return memoryStream;
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar arquivo do S3: {BucketName}/{Key}. Código: {ErrorCode}, Mensagem: {ErrorMessage}",
                _bucketName, key, ex.ErrorCode, ex.Message);
            throw;
        }
    }



    public async Task<bool> FileExistsAsync(string key)
    {
        try
        {
            var request = new GetObjectMetadataRequest
            {
                BucketName = _bucketName,
                Key = key,
            };
            await _s3Client.GetObjectMetadataAsync(request);
            return true;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar existência do arquivo: {BucketName}/{Key}", _bucketName, key);
            throw;
        }
    }

    public async Task SaveFileAsync(string key, Stream content, string contentType = null)
    {
        try
        {
            _logger.LogInformation("Salvando arquivo no S3: {BucketName}/{Key}", _bucketName, key);

            var request = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = key,
                InputStream = content
            };

            if (!string.IsNullOrEmpty(contentType))
            {
                request.ContentType = contentType;
            }

            await _s3Client.PutObjectAsync(request);

            _logger.LogInformation("Arquivo salvo com sucesso: {BucketName}/{Key}", _bucketName, key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao salvar arquivo no S3: {BucketName}/{Key}", _bucketName, key);
            throw;
        }
    }
}