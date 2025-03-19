using Amazon;
using Amazon.S3;
using Amazon.SQS;
using FluxoDeCaixa.Consolidacao.Cloud.AWS;
using FluxoDeCaixa.Consolidacao.Common.Configuration;
using FluxoDeCaixa.Consolidacao.Domain.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace FluxoDeCaixa.Consolidacao.IoC.ModuleInitializers;

public class CloudModuleInitializer : IModuleInitializer
{
    public void Initialize(HostApplicationBuilder builder)
    {

        builder.Services.Configure<SqsConfig>(builder.Configuration.GetSection("SqsConfig"));
        builder.Services.Configure<S3Config>(builder.Configuration.GetSection("S3Config"));
        RegisterDependencies(ref builder);
    }

    private void RegisterDependencies(ref HostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IS3Service, S3Service>();
        // Register AWS Clients
        builder.Services.AddSingleton<IAmazonSQS>(sp => {
            var awsSettings = sp.GetRequiredService<IOptions<SqsConfig>>().Value;
            var sqsConfig = new AmazonSQSConfig
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(awsSettings.Region)
            };

            return new AmazonSQSClient(
                awsSettings.AccessKey,
                awsSettings.SecretKey,
                sqsConfig);
        });

        builder.Services.AddSingleton<IAmazonS3>(sp => {
            var awsSettings = sp.GetRequiredService<IOptions<S3Config>>().Value;
            var s3Config = new AmazonS3Config
            {
#if DEBUG
                // Necessário para LocalStack 
                ServiceURL = "http://localhost:4566",
                ForcePathStyle = true
#else
                RegionEndpoint = RegionEndpoint.GetBySystemName(awsSettings.Region)
#endif
            };

            return new AmazonS3Client(
                awsSettings.AccessKey,
                awsSettings.SecretKey,
                s3Config);
        });

    }
}

