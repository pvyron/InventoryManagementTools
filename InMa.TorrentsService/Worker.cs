using InMa.Torrents;

namespace InMa.TorrentsService;

public class TorrentsService : BackgroundService
{
    private readonly ILogger<TorrentsService> _logger;
    private readonly TorrentDownloadService _torrentDownloadService;

    public TorrentsService(ILogger<TorrentsService> logger, TorrentDownloadService torrentDownloadService)
    {
        _logger = logger;
        _torrentDownloadService = torrentDownloadService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }

            await Task.Delay(1000, stoppingToken);
        }
    }
}