using Amazon.SQS;
using Amazon.SQS.Model;
using FluxoDeCaixa.Consolidacao.Application.Interfaces;
using FluxoDeCaixa.Consolidacao.Common.Configuration;
using Microsoft.Extensions.Options;
namespace FluxoDeCaixa.Consolidacao.Worker;

public class SqsConsumerWorker : BackgroundService
{
    private readonly ILogger<SqsConsumerWorker> _logger;
    private readonly SqsConfig _sqsConfig;
    private readonly IAmazonSQS _sqsClient;
    private readonly ICashRegisterAppService _appService;

    public SqsConsumerWorker(ICashRegisterAppService appService, IAmazonSQS sqsClient, ILogger<SqsConsumerWorker> logger, IOptions<SqsConfig> sqsConfig)
    {
        _logger = logger;
        _appService = appService;
        _sqsConfig = sqsConfig.Value;
        _sqsClient = sqsClient;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SQS Consumer Service iniciado em: {time}", DateTimeOffset.Now);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var receiveMessageRequest = new ReceiveMessageRequest
                {
                    QueueUrl = _sqsConfig.QueueUrl,
                    MaxNumberOfMessages = _sqsConfig.MaxNumberOfMessages,
                    WaitTimeSeconds = _sqsConfig.WaitTimeSeconds // Long polling
                };
                var response = await _sqsClient.ReceiveMessageAsync(receiveMessageRequest, stoppingToken);

                foreach (var message in response.Messages)
                {
                    try
                    {
                        _logger.LogInformation("Mensagem recebida: {MessageId}", message.MessageId);
                        _logger.LogInformation("Conteúdo: {Body}", message.Body);

                        // Processa a mensagem aqui
                        await ProcessMessageAsync(message);

                        // Apaga a mensagem da fila após processamento
                        await _sqsClient.DeleteMessageAsync(
                            _sqsConfig.QueueUrl,
                            message.ReceiptHandle,
                            stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Erro ao processar mensagem: {MessageId}", message.MessageId);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao receber mensagens do SQS");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
    private Task ProcessMessageAsync(Message message)
    {
        _appService.CalculateBalance(message.Body);
        _logger.LogInformation("Processando mensagem: {MessageId}", message.MessageId);
        return Task.CompletedTask;
    }
    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SQS Consumer Service parando em: {time}", DateTimeOffset.Now);
        await base.StopAsync(stoppingToken);
    }
}
