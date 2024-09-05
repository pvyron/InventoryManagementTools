using Microsoft.Extensions.DependencyInjection;

namespace InMa.Torrents;

public static class Extensions
{
    public static IServiceCollection AddTorrents(this IServiceCollection services, string azureStorageConnectionString = "")
    {
        services.AddHttpClient("torrent-search", client =>
        {
            client.BaseAddress = new Uri("https://apibay.org/q.php");
        });
        
        services.AddSingleton<TorrentSearchService>();
        services.AddSingleton<TorrentDownloadService>();

        return services;
    }
    
    static readonly string FormatTemplate = "{0:0.00} {1}";
    static readonly string[] Units = ["Bytes", "kB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"];
    
    internal static string BytesAsText(this string? fileSizeBytesText)
    {
        if (string.IsNullOrWhiteSpace(fileSizeBytesText))
        {
            return "0 Bytes";
        }

        if (!long.TryParse(fileSizeBytesText, out long fileSizeBytes))
        {
            return "0 Bytes";
        }

        return fileSizeBytes.BytesAsText();
    }
    
    internal static string BytesAsText(this long fileSizeBytes)
    {
        if (fileSizeBytes <= 0)
        {
            return string.Format(FormatTemplate, 0, Units[0]);
        }
        
        var index = Math.Log(fileSizeBytes, 1024);
        
        var indexNorm = index > Units.Length ? Units.Length - 1 : (int)index;
        
        var value = fileSizeBytes / Math.Pow(1024, indexNorm);
        
        return string.Format(FormatTemplate, value, Units[indexNorm]);
    }
}