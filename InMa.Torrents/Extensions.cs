using Microsoft.Extensions.DependencyInjection;

namespace InMa.Torrents;

public static class Extensions
{
    public static IServiceCollection AddTorrents(this IServiceCollection services)
    {
        services.AddHttpClient("torrent-search", client =>
        {
            client.BaseAddress = new Uri("https://apibay.org/q.php");
        });
        
        services.AddSingleton<TorrentSearchService>();
        services.AddSingleton<TorrentDownloadService>();

        return services;
    }
}