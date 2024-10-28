using Serilog;

Console.Title = "Server-2, send-email";

// Настройка Serilog для логирования в консоль и файл
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Host.UseSerilog(); // Подключаем Serilog как основной логгер

builder.Services.AddControllers();

// Настройка CORS для разрешения запросов с указанного источника
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClientApp", policy =>
    {
        policy.WithOrigins("http://127.0.0.1:5500")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();
app.UseCors("AllowClientApp"); // Применяем политику CORS глобально

// Эндпоинт для проверки новых email-сообщений
app.MapGet("/check-emails", () =>
{
    Log.Information("Received request to check emails.");

    // Проверка на наличие новых данных
    var emails = GetNewEmails();
    if (emails.Any())
    {
        Log.Information("Returning new emails to the client.");
        return Results.Ok(emails);
    }
    else
    {
        Log.Information("No new emails found. Returning 204 No Content.");
        return Results.NoContent();
    }
});

// Эндпоинт для обработки отправки email (получение данных от Server 1)
app.MapPost("/send-email", (EmailRequest request) =>
{
    Log.Information("Received email request. Sending email to: {Email}", request.Email);
    Console.WriteLine($"Email sent to: {request.Email} with message: {request.Message}");
    return Results.Ok("Email sent successfully");
});

app.Run("http://localhost:5002");

// Метод для проверки новых email-сообщений
List<EmailMessage> GetNewEmails()
{
    // Реализуйте логику для проверки наличия новых email, например, обращение к базе данных
    // В данном примере просто возвращается один элемент или пустой список для демонстрации
    return new List<EmailMessage>
    {
        new EmailMessage { Email = "test@example.com", Message = "Webhook triggered!" }
    };
}

// Классы для обработки email-сообщений
public record EmailRequest(string Email, string Message);
public class EmailMessage
{
    public string Email { get; set; }
    public string Message { get; set; }
}
