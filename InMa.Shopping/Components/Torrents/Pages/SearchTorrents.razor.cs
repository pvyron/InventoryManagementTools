using InMa.Torrents;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace InMa.Shopping.Components.Torrents.Pages;

public partial class SearchTorrents
{
    [Inject] private TorrentSearchService TorrentSearchService { get; set; } = null!;
    [Inject] private TorrentDownloadService TorrentDownloadService { get; set; } = null!;

    private IQueryable<Torrent>? Torrents { get; set; } = Enumerable.Empty<Torrent>().AsQueryable();
    private string Query { get; set; } = string.Empty;
    private bool Searching { get; set; } = false;

    private async Task SearchButtonClicked(MouseEventArgs obj)
    {
        if (string.IsNullOrWhiteSpace(Query))
            return;
        
        Torrents = (await TorrentSearchService.Search(Query, CancellationToken.None)).AsQueryable();
    }

    private async Task DownloadTorrent(Torrent torrent)
    {
        await TorrentDownloadService.QueueTorrentForDownload(torrent);
    }
}