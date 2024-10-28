using System.Net;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace long_polling_client
{
    public class LongPollingWorker : BackgroundService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<LongPollingWorker> _logger;
        private readonly string _url = "https://localhost:57910/long-polling"; // Укажите правильный URL

        public LongPollingWorker(HttpClient httpClient, ILogger<LongPollingWorker> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Long polling worker started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var response = await _httpClient.GetAsync(_url, stoppingToken);

                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        HandleNewData(data);
                    }
                    else if (response.StatusCode == HttpStatusCode.NoContent)
                    {
                        _logger.LogInformation("No new data available, retrying...");
                        await Task.Delay(5000, stoppingToken); // Wait before the next poll
                    }
                    else
                    {
                        HandleError(response.StatusCode);
                    }
                }
                catch (HttpRequestException httpEx)
                {
                    HandleError(httpEx);
                }
                catch (TaskCanceledException)
                {
                    _logger.LogWarning("Request timed out. Retrying...");
                }
                catch (Exception ex)
                {
                    HandleError(ex);
                }
            }

            _logger.LogInformation("Long polling worker stopped.");
        }

        private void HandleNewData(string data)
        {
            _logger.LogInformation($"New data received: {data}");
        }

        private void HandleError(HttpStatusCode statusCode)
        {
            _logger.LogError($"HTTP Error: {statusCode}");
        }

        private void HandleError(Exception ex)
        {
            _logger.LogError($"Exception: {ex.Message}");
        }
    }
}
