using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using Serilog;
using Timer = System.Threading.Timer;
using Microsoft.AspNetCore.Http;

class Program
{
    private static Timer _messageTimer;
    private static ConcurrentQueue<string> _messageQueue = new ConcurrentQueue<string>();
    private static int _messageCount = 1; // Счетчик сообщений

    static void Main(string[] args)
    {
        // Настройка Serilog
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .CreateLogger();

        var host = new WebHostBuilder()
            .UseKestrel()
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Warning);
            })
            .ConfigureServices(services =>
            {
                services.AddCors(options =>
                {
                    options.AddPolicy("AllowSpecificOrigin", builder =>
                    {
                        builder.WithOrigins("http://127.0.0.1:5500")
                               .AllowAnyHeader()
                               .AllowAnyMethod();
                    });
                });
            })
            .Configure(app =>
            {
                app.UseCors("AllowSpecificOrigin");
                app.Run(async context =>
                {
                    if (context.Request.Path == "/get-data")
                    {
                        var messages = new List<string>();
                        while (_messageQueue.TryDequeue(out string message))
                        {
                            messages.Add(message);
                        }

                        // Возвращаем пустой массив, если сообщений нет
                        if (messages.Count > 0)
                        {
                            var jsonResponse = System.Text.Json.JsonSerializer.Serialize(messages);
                            context.Response.ContentType = "application/json";
                            await context.Response.WriteAsync(jsonResponse);
                        }
                        else
                        {
                            context.Response.ContentType = "application/json"; // Убедитесь, что это установлен
                            await context.Response.WriteAsync("[]"); // Возвращаем пустой массив
                        }
                    }
                    else
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    }
                });
            })
            .UseUrls("http://localhost:57910")
            .Build();

        StartMessageGeneration();

        host.Run();
    }

    private static void StartMessageGeneration()
    {
        _messageTimer = new Timer(_ =>
        {
            string message = $"Сообщение #{_messageCount++} от {DateTime.Now}";
            _messageQueue.Enqueue(message);
            Log.Information($"Сгенерировано сообщение: {message}");
        }, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10)); // Задержка в 1 секунду
    }
}
