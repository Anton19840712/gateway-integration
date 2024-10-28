using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Настройка Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration) // Чтение конфигурации из appsettings.json
    .WriteTo.Console() // Логирование в консоль
    .CreateLogger();

Console.Title = "http-https-server";

// Добавляем сервисы
builder.Host.UseSerilog(); // Используем Serilog как логгер

var services = builder.Services;

// Добавляем CORS
services.AddCors();

// Добавляем фоновые службы
services.AddHostedService<BackgroundMessageProcessor>();

var app = builder.Build();

// Логируем, на каких адресах запущен сервер
var urls = builder.WebHost.GetSetting("urls");
Log.Information($"Server is running on: {urls}");

// Используем CORS
app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
);

// POST-запрос для получения сообщений
app.MapPost("/api/messages", async (HttpContext context) =>
{
    using var reader = new StreamReader(context.Request.Body);
    var message = await reader.ReadToEndAsync();

    // Определяем, по какому протоколу пришел запрос
    var requestProtocol = context.Request.Scheme;

    // Логируем информацию о сообщении и протоколе
    Log.Information($"Received {requestProtocol.ToUpper()} message: {message}");

    return Results.Ok();
});

// Запускаем приложение
app.Run();

// Фоновая служба для обработки сообщений
public class BackgroundMessageProcessor : BackgroundService
{
    private readonly ILogger<BackgroundMessageProcessor> _logger;

    public BackgroundMessageProcessor(ILogger<BackgroundMessageProcessor> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Background message processor started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(10000, stoppingToken);
        }

        _logger.LogInformation("Background message processor is stopping.");
    }
}
