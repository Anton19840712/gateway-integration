using FluentFTP;
using Serilog;

namespace ftps_client
{
    public class FtpService
    {
        public void Start()
        {
            // Настройка Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug() // Уровень логирования
                .WriteTo.Console() // Запись логов в консоль
                .CreateLogger();

            // Конфигурация подключения
            string url = "ftp://127.0.0.1"; // URL FTP-сервера
            string filePath = "/documents1"; // Путь к файлу на FTP-сервере
            string user = "antontest"; // Имя пользователя для входа на FTP-сервер
            string password = "13"; // Пароль для входа на FTP-сервер

            using (var client = new FtpClient("127.0.0.1", user, password))
            {
                client.Port = 21; // Порт для FTPS
                client.EncryptionMode = FtpEncryptionMode.Explicit; // Использовать явное шифрование
                client.ValidateCertificate += new FtpSslValidation((control, e) =>
                {
                    e.Accept = true; // Принять все сертификаты (только для тестов)
                });

                try
                {
                    // Подключаемся к серверу
                    Log.Information("Подключение к серверу...");
                    client.Connect();
                    Log.Information("Подключение успешно выполнено.");

                    // Пример загрузки файла на сервер
                    string localFilePath = @"C:\Documents2\test4.html"; // папка на твоей машине
                    Log.Information($"Загрузка файла {localFilePath} на сервер...");
                    client.UploadFile(localFilePath, "/documents1/test4.html"); // серверная папка
                    Log.Information("Файл успешно загружен на сервер.");

                    // Пример скачивания файла с сервера
                    string downloadedFilePath = @"C:\Documents2\test.html"; // путь для сохранения скачанного файла на твоей машине
                    Log.Information($"Скачивание файла с сервера в {downloadedFilePath}...");
                    client.DownloadFile(downloadedFilePath, "/documents1/test.html"); // путь для скачивания на твоей машине
                    Log.Information("Файл успешно скачан с сервера.");
                }
                catch (Exception ex)
                {
                    Log.Error($"Ошибка: {ex.Message}");
                }
                finally
                {
                    // Отключаемся от сервера
                    if (client.IsConnected)
                    {
                        client.Disconnect();
                        Log.Information("Отключение от сервера выполнено.");
                    }
                }
            }

            // Завершаем логирование
            Log.CloseAndFlush();
        }
    }
}
