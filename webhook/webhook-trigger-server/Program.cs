// Server1.cs

using Serilog;

Console.Title = "Server-1, trigger-webhook";
var builder = WebApplication.CreateBuilder(args);

// Настройка Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(Log.Logger); // Используем Serilog как логгер
builder.Services.AddControllers();

// Configure CORS to allow requests from your client’s origin
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClientApp", policy =>
    {
        policy.WithOrigins("http://127.0.0.1:5500")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Register HttpClient for dependency injection
builder.Services.AddHttpClient();

var app = builder.Build();

// Apply the CORS policy globally
app.UseCors("AllowClientApp");

// Map the webhook endpoint
app.MapPost("/trigger-webhook", async (HttpClient httpClient) =>
{
    var payload = new { email = "test@example.com", message = "Webhook triggered!" };

    try
    {
        // Send POST request to Server 2 - это и будет так называемый webhook
        var response = await httpClient.PostAsJsonAsync("http://localhost:5002/send-email", payload);

        if (response.IsSuccessStatusCode)
        {
            Log.Information("Webhook sent successfully to Server 2.");
            return Results.Ok("Webhook sent to Server 2");
        }
        else
        {
            // Log the response status if the request failed
            Log.Error("Failed to send webhook. Status Code: {StatusCode}", response.StatusCode);
            return Results.StatusCode((int)response.StatusCode);
        }
    }
    catch (Exception ex)
    {
        // Log any exceptions that occur
        Log.Error(ex, "An error occurred while sending the webhook to Server 2.");
        return Results.StatusCode(500);
    }
});

app.Run("http://localhost:5001");

// Очищаем и закрываем логгер при завершении работы
AppDomain.CurrentDomain.ProcessExit += (sender, eventArgs) => Log.CloseAndFlush();
