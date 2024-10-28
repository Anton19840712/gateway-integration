using Serilog;

class Program
{
    public static async Task Main(string[] args)
    {
        // Настройка Serilog
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .CreateLogger();

        try
        {
            Log.Information("Starting up the host");

            // Создание хоста приложения
            var host = Host.CreateDefaultBuilder(args)
                .UseSerilog() // Используем Serilog как логгер
                .ConfigureServices((hostContext, services) =>
                {
                    // Регистрируем DataGenerationService как фоновый сервис
                    services.AddHostedService<DataGenerationService>();
                })
                .Build();

            await host.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
