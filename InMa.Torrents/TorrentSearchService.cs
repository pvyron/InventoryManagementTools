using System.Net.Http.Json;
using System.Text.Json;
using System.Web;

namespace InMa.Torrents;

public class TorrentSearchService
{
    private readonly HttpClient _httpClient;

    public TorrentSearchService(IHttpClientFactory httpClientFactory, TorrentSettings torrentSettings)
    {
        _httpClient = httpClientFactory.CreateClient();
        _httpClient.BaseAddress = new Uri(torrentSettings.SearchApiUrl);
    }
    public async ValueTask<Torrent[]> Search(string query, CancellationToken cancellationToken)
    {
        var encodedQuery = HttpUtility.UrlEncode(query);

        return await _httpClient.GetFromJsonAsync<Torrent[]>($"?q={encodedQuery}", new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        }, cancellationToken) ?? [];
    }
}