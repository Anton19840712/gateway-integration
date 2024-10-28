using ftps_client;
using Serilog;

// Настройка Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug() // Уровень логирования
    .WriteTo.Console() // Запись логов в консоль
    .CreateLogger();

// Запуск фонового процесса
await Task.Run(() =>
{
    var ftpService = new FtpService();
    ftpService.Start();
});

// Ваш основной код, если необходимо
Console.WriteLine("Нажмите любую клавишу для выхода...");
Console.ReadKey();

// Завершение логирования
Log.CloseAndFlush();
