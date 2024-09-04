using System.Text.Json.Serialization;

namespace InMa.Torrents;

public record class Torrent
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    [JsonPropertyName("info_hash")]
    public string? InfoHash { get; set; }
    public string? Leechers { get; set; }
    public string? Seeders { get; set; }
    public string? NumFiles { get; set; }
    public string? Size { get; set; }
    public string? Username { get; set; }
    public string? Added { get; set; }
    public string? Status { get; set; }
    public string? Category { get; set; }
    public string? Imdb { get; set; }

    public string MagnetUrl => $"magnet:?xt=urn:btih:{InfoHash ?? ""}";
    public string SizeText => Size != null ? $"{Size}B" : string.Empty;
}