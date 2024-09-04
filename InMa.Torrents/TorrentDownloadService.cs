using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;

namespace InMa.Torrents;

public sealed class TorrentDownloadService
{
    private readonly QueueClient _queueClient;
    
    public TorrentDownloadService(IConfiguration configuration)
    {
        _queueClient = new QueueClient(configuration.GetConnectionString("StorageAccount"), configuration["Torrents:QueueName"]);
    }
    
    public async ValueTask QueueTorrentForDownload(Torrent torrent)
    {
        if (string.IsNullOrWhiteSpace(torrent.InfoHash))
            return;

        await _queueClient.SendMessageAsync(torrent.MagnetUrl);
    }
    
    public async Task Initialize()
    {
        await _queueClient.CreateIfNotExistsAsync();
    }
}