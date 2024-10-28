using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Net.Sockets;
using System.Net;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        Console.Title = "Server";

        // Настройка Serilog
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console() // Логирование в консоль
            .CreateLogger();

        // Создание хоста
        var host = Host.CreateDefaultBuilder(args)
            .UseSerilog() // Использование Serilog
            .ConfigureServices((context, services) =>
            {
                // Регистрация сервиса UDP
                services.AddSingleton<UDPServer>();
            })
            .Build();

        // Запуск сервера
        host.Services.GetRequiredService<UDPServer>().Start();
    }
}

// Класс для UDP-сервера
public class UDPServer
{
    private readonly ILogger<UDPServer> _logger;

    public UDPServer(ILogger<UDPServer> logger)
    {
        _logger = logger;
    }

    public void Start()
    {
        // создаем клиента, чтобы получить данные
        using UdpClient listener = new UdpClient(5000);
        IPEndPoint EP = new IPEndPoint(IPAddress.Any, 5000);

        try
        {
            _logger.LogInformation("Waiting for messages...");
            while (true)
            {
                byte[] receivedbytes = listener.Receive(ref EP);

                string data = Encoding.ASCII.GetString(receivedbytes, 0, receivedbytes.Length);
                _logger.LogInformation($"Received message from client: {data}");
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while receiving data");
        }
    }
}
