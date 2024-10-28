using System.Net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Threading;
using Serilog;

class Program
{
    private static CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
    private static bool _isRunning = false;
    private static ConcurrentQueue<string> _messageQueue = new ConcurrentQueue<string>(); // Очередь для хранения сообщений
    private static bool _isClientConnected = false;
    private static Timer _timer; // Таймер для отсчета времени
    private static Timer _clientDisconnectTimer; // Таймер для отслеживания отключения клиента

    static void Main(string[] args)
    {
        Console.Title = "Server";

        // Настройка Serilog
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .CreateLogger();

        var host = WebHost.CreateDefaultBuilder(args)
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole(options =>
                {
                    options.LogToStandardErrorThreshold = LogLevel.Error;
                });
                logging.SetMinimumLevel(LogLevel.Warning);
            })
            .ConfigureServices(services =>
            {
                services.AddCors(options =>
                {
                    options.AddPolicy("AllowSpecificOrigin",
                        builder =>
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
                        // Обработка подключения клиента
                        _isClientConnected = true;

                        if (_clientDisconnectTimer != null)
                        {
                            _clientDisconnectTimer.Dispose(); // Остановить таймер отключения, если он активен
                        }

                        if (_messageQueue.TryDequeue(out string latestMessage))
                        {
                            await context.Response.WriteAsync(latestMessage);
                        }
                        else
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.NoContent;
                        }

                        // Запускаем таймер отключения клиента на 5 секунд
                        _clientDisconnectTimer = new Timer(ClientDisconnectCallback, null, TimeSpan.FromSeconds(5), TimeSpan.FromMilliseconds(-1));
                    }
                    else
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.NoContent;
                    }
                });
            })
            .UseUrls("http://localhost:57910")
            .Build();

        _ = host.RunAsync();

        Log.Information("The application is running. Enter the command (start <seconds>,<rps>, stop, exit):");

        while (true)
        {
            var command = Console.ReadLine()?.ToLower();
            if (command.StartsWith("start"))
            {
                var parts = command.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 3 && int.TryParse(parts[1], out int durationInSeconds) && int.TryParse(parts[2], out int rps))
                {
                    if (!_isRunning)
                    {
                        _isRunning = true;
                        _cancellationTokenSource = new CancellationTokenSource();
                        _ = StartProcessAsync(_cancellationTokenSource.Token, durationInSeconds, rps);
                    }
                    else
                    {
                        Log.Information("The process has already started...");
                    }
                }
                else
                {
                    Log.Information("Invalid command format. Use 'start <seconds>,<rps>'.");
                }
                continue;
            }

            switch (command)
            {
                case "stop":
                    if (_isRunning)
                    {
                        _cancellationTokenSource.Cancel();
                        _isRunning = false;
                    }
                    else
                    {
                        Log.Information("The process was not started...");
                    }
                    break;
                case "exit":
                    if (_isRunning) _cancellationTokenSource.Cancel();
                    return;
            }
        }
    }

    private static async Task StartProcessAsync(CancellationToken token, int durationInSeconds, int rps)
    {
        Log.Information("Data generation process starts.");
        int i = 1;
        int delayInMilliseconds = 1000 / rps;
        using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(durationInSeconds));
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, timeoutCts.Token);

        while (!linkedCts.Token.IsCancellationRequested)
        {
            string message = $"Data - {i++} generated at {DateTime.Now}";
            _messageQueue.Enqueue(message);

            try
            {
                await Task.Delay(delayInMilliseconds, linkedCts.Token);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }
        Log.Information("Data generation expired.");

        StartTimer();

        await Task.Delay(Timeout.Infinite, linkedCts.Token);
    }

    private static void StartTimer()
    {
        _timer?.Dispose();

        _timer = new Timer(_ =>
        {
            if (!_isClientConnected)
            {
                Log.Information("10 seconds have expired. Displaying unread messages...");

                while (_messageQueue.TryDequeue(out string unreadMessage))
                {
                    Log.Warning($"Unread Message: {unreadMessage}");
                }

                _timer.Dispose();
            }
        }, null, TimeSpan.FromSeconds(10), TimeSpan.FromMilliseconds(-1));
    }

    private static void ClientDisconnectCallback(object state)
    {
        _isClientConnected = false;

        StartTimer();
    }
}
