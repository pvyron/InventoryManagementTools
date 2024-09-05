namespace InMa.Torrents;

public sealed class TorrentSettings
{
    public string SearchApiUrl { get; set; } = "https://apibay.org/q.php";
    public string ClientApiUrl { get; set; } = "http://localhost:8088";
    public required string StorageAccountConnectionString { get; set; }
    public string QueueName { get; set; } = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") == "Production" ? "torpending" : "tordemopending";
}