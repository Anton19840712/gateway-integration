public class DataGenerationService : BackgroundService
{
    private readonly ILogger<DataGenerationService> _logger;
    private CancellationTokenSource _cancellationTokenSource;

    public DataGenerationService(ILogger<DataGenerationService> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
        _logger.LogInformation("Data generation service started at: {time}", DateTimeOffset.Now);

        while (!_cancellationTokenSource.Token.IsCancellationRequested)
        {
            var generatedData = $"Generated data at {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
            _logger.LogInformation(generatedData);

            await Task.Delay(1000, _cancellationTokenSource.Token);
        }

        _logger.LogInformation("Data generation service stopped at: {time}", DateTimeOffset.Now);
    }

    public void Stop()
    {
        _cancellationTokenSource.Cancel();
    }
}
