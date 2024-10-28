using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Net.Sockets;
using System.Net;
using System.Text;

class Program
{
    static async Task Main(string[] args)
    {

        Console.Title = "Client";

        // Настройка Serilog
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .CreateLogger();

        // Создание хоста
        var host = Host.CreateDefaultBuilder(args)
            .UseSerilog() // Использование Serilog
            .ConfigureServices((context, services) =>
            {
                // Регистрация фоновой службы
                services.AddHostedService<UDPClientService>();
            })
            .Build();

        // Запуск приложения
        await host.RunAsync();
    }
}

// Фоновая служба для UDP-клиента
public class UDPClientService : BackgroundService
{
    private readonly ILogger<UDPClientService> _logger;
    private Socket _socket;
    private IPEndPoint _endpoint;

    public UDPClientService(ILogger<UDPClientService> logger)
    {
        _logger = logger;

        // Настройка сокета и конечной точки
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        IPAddress sendto = IPAddress.Parse("127.0.0.1");
        _endpoint = new IPEndPoint(sendto, 5000);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        int i = 1;
        while (!stoppingToken.IsCancellationRequested)
        {
            string texttosend = $"test-message-from-client {i++}";
            byte[] sendbuffer = Encoding.ASCII.GetBytes(texttosend);

            _logger.LogInformation($"Trying to send to: {IPAddress.Any} port: 5000 message like {texttosend}");

            try
            {
                _socket.SendTo(sendbuffer, _endpoint);
                _logger.LogInformation($"Sent message to a server");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception occurred while sending message");
            }

            // Пауза перед следующей отправкой
            await Task.Delay(5000, stoppingToken); // Отправка сообщения каждые 5 секунд
        }
    }

    public override void Dispose()
    {
        _socket?.Close();
        base.Dispose();
    }
}
