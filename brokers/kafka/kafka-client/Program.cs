using kafka_client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

Console.Title = "consumer";

CreateHostBuilder(args).Build().Run();

static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .UseSerilog((context, services, configuration) =>
        {
            configuration
                .WriteTo.Console()
                .ReadFrom.Configuration(context.Configuration); // Чтение конфигурации из appsettings.json, если есть
        })
        .ConfigureServices((context, collection) =>
        {
            collection.AddHostedService<KafkaConsumerHostedService>();
        });
