namespace FluxoDeCaixa.Consolidacao.Common.Configuration;

/// <summary>
/// Configuração do publicador SQS
/// </summary>
public class SqsConfig
{
    public string QueueUrl { get; set; }
    public string Region { get; set; }
    public string AccessKey { get; set; }
    public string SecretKey { get; set; }
    public int MaxNumberOfMessages { get; set; } = 10;
    public int WaitTimeSeconds { get; set; } = 20;
}