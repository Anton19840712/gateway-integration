using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ��������� Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration) // ������ ������������ �� appsettings.json
    .WriteTo.Console() // ����������� � �������
    .CreateLogger();

Console.Title = "http-https-server";

// ��������� �������
builder.Host.UseSerilog(); // ���������� Serilog ��� ������

var services = builder.Services;

// ��������� CORS
services.AddCors();

// ��������� ������� ������
services.AddHostedService<BackgroundMessageProcessor>();

var app = builder.Build();

// ��������, �� ����� ������� ������� ������
var urls = builder.WebHost.GetSetting("urls");
Log.Information($"Server is running on: {urls}");

// ���������� CORS
app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
);

// POST-������ ��� ��������� ���������
app.MapPost("/api/messages", async (HttpContext context) =>
{
    using var reader = new StreamReader(context.Request.Body);
    var message = await reader.ReadToEndAsync();

    // ����������, �� ������ ��������� ������ ������
    var requestProtocol = context.Request.Scheme;

    // �������� ���������� � ��������� � ���������
    Log.Information($"Received {requestProtocol.ToUpper()} message: {message}");

    return Results.Ok();
});

// ��������� ����������
app.Run();

// ������� ������ ��� ��������� ���������
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
