using System.Net.Sockets;
using System.Text;
using System.Xml.Serialization;
using tcp_client.Models;

class TcpClientExample
{
    private static async Task SendMessage(string serverIp, int port, string message)
    {
        try
        {
            using (TcpClient client = new TcpClient(serverIp, port))
            using (NetworkStream stream = client.GetStream())
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                await stream.WriteAsync(data, 0, data.Length);
                Console.WriteLine($"Message sent: {message}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private static string SerializeToXml<T>(T obj)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        using (StringWriter writer = new StringWriter())
        {
            serializer.Serialize(writer, obj);
            return writer.ToString();
        }
    }

    public static async Task StartClient(string serverIp, int port)
    {
        // Создание экземпляра объекта для отправки
        var cardRequest = new Card112ChangedRequest
        {
            GlobalId = Guid.NewGuid().ToString(),
            EmergencyCardId = 12345,
            DtCreate = DateTime.Now,
            CallTypeId = 1,
            CardSyntheticState = 0,
            WithCall = 1,
            Creator = "John Doe",
            AddressLevel1 = "Main Street",
            IncidentDescription = "Sample incident description"
            // Заполните остальные поля, если нужно
        };

        var soapEnvelope = new SoapEnvelope
        {
            Body = new SoapBody
            {
                Card112ChangedRequest = cardRequest
            }
        };

        // Сериализация в XML
        string xmlMessage = SerializeToXml(soapEnvelope);

        // Симуляция отправки данных через TCP каждые 2 секунды
        while (true)
        {
            await SendMessage(serverIp, port, xmlMessage);
            await Task.Delay(2000); // Ожидание 2 секунды перед следующей отправкой
        }
    }

    static async Task Main(string[] args)
    {
        string serverIp = "127.0.0.1"; // IP-адрес сервера
        int port = 6295; // Порт сервера

        await StartClient(serverIp, port);
    }
}