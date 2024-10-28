using Serilog;

Console.Title = "Server-2, send-email";

// ��������� Serilog ��� ����������� � ������� � ����
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Host.UseSerilog(); // ���������� Serilog ��� �������� ������

builder.Services.AddControllers();

// ��������� CORS ��� ���������� �������� � ���������� ���������
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
app.UseCors("AllowClientApp"); // ��������� �������� CORS ���������

// �������� ��� �������� ����� email-���������
app.MapGet("/check-emails", () =>
{
    Log.Information("Received request to check emails.");

    // �������� �� ������� ����� ������
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

// �������� ��� ��������� �������� email (��������� ������ �� Server 1)
app.MapPost("/send-email", (EmailRequest request) =>
{
    Log.Information("Received email request. Sending email to: {Email}", request.Email);
    Console.WriteLine($"Email sent to: {request.Email} with message: {request.Message}");
    return Results.Ok("Email sent successfully");
});

app.Run("http://localhost:5002");

// ����� ��� �������� ����� email-���������
List<EmailMessage> GetNewEmails()
{
    // ���������� ������ ��� �������� ������� ����� email, ��������, ��������� � ���� ������
    // � ������ ������� ������ ������������ ���� ������� ��� ������ ������ ��� ������������
    return new List<EmailMessage>
    {
        new EmailMessage { Email = "test@example.com", Message = "Webhook triggered!" }
    };
}

// ������ ��� ��������� email-���������
public record EmailRequest(string Email, string Message);
public class EmailMessage
{
    public string Email { get; set; }
    public string Message { get; set; }
}
