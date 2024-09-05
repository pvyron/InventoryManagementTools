using System.Net.Http.Json;
using InMa.Torrents;

namespace InMa.TorrentsService;

public class TorrentsService : BackgroundService
{
    private readonly ILogger<TorrentsService> _logger;
    private readonly TorrentDownloadService _torrentDownloadService;
    private readonly HttpClient _httpClient;
    
    public TorrentsService(ILogger<TorrentsService> logger, TorrentDownloadService torrentDownloadService, TorrentSettings torrentSettings, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _torrentDownloadService = torrentDownloadService;
        _httpClient = httpClientFactory.CreateClient();
        _httpClient.BaseAddress = new Uri(torrentSettings.ClientApiUrl);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (!await IsTorrentClientRunning())
            {
                await Task.Delay(5000, stoppingToken);
                continue;
            }
            
            var magnetLinks = await _torrentDownloadService.GetTorrentsForDownload();
            
            var responses = await DownloadTorrents(magnetLinks);
            
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Responses: {responses}", string.Join(" | ", responses));
            }

            await Task.Delay(5000, stoppingToken);
        }
    }

    private async ValueTask<bool> IsTorrentClientRunning()
    {
        var processes = System.Diagnostics.Process.GetProcesses();

        if (processes.Any(p => p.ProcessName.Equals("qbittorrent", StringComparison.OrdinalIgnoreCase)))
            return true;

        System.Diagnostics.Process.Start("\"C:\\Program Files\\qBittorrent\\qbittorrent.exe\"");

        await Task.Delay(10000);
        
        return true;
    }

    private async ValueTask<bool[]> DownloadTorrents(string[] magnetLinks)
    {
        var result = new bool[magnetLinks.Length];

        for (int i = 0; i < magnetLinks.Length; i++)
        {
            result[i] = await DownloadTorrent(magnetLinks[i]);
        }
        
        return result;
    }

    private async ValueTask<bool> DownloadTorrent(string magnetLink)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v2/torrents/add");
        var content = new MultipartFormDataContent();
        content.Add(new StringContent(magnetLink), "urls");
        request.Content = content;
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        
        var textResponse = await response.Content.ReadAsStringAsync();
        
        _logger.LogInformation("Request for magnet: {magnetLink} | response: {response}", magnetLink, textResponse);

        return textResponse.Contains("ok", StringComparison.OrdinalIgnoreCase);
    }
}