using long_polling_client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

public class Program
{
    public static void Main(string[] args)
    {
        Console.Title = "http-https-client";
        // Настройка Serilog
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console()  // Логирование в консоль
            .CreateLogger();

        try
        {
            Log.Information("Starting long-polling client...");
            CreateHostBuilder(args).Build().Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Client terminated unexpectedly.");
        }
        finally
        {
            Log.CloseAndFlush(); // Очистка и закрытие логгера
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog()  // Заменить стандартный логгер на Serilog
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHttpClient<LongPollingWorker>(); // Register HttpClient
                services.AddHostedService<LongPollingWorker>(); // Register the long polling worker
            });
}
