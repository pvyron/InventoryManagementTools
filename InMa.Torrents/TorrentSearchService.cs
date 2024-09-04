using System.Net.Http.Json;
using System.Text.Json;
using System.Web;

namespace InMa.Torrents;

public class TorrentSearchService(IHttpClientFactory httpClientFactory)
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("torrent-search");

    public async ValueTask<Torrent[]> Search(string query, CancellationToken cancellationToken)
    {
        var encodedQuery = HttpUtility.UrlEncode(query);

        return await _httpClient.GetFromJsonAsync<Torrent[]>($"?q={encodedQuery}", new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        }, cancellationToken) ?? [];
    }
}