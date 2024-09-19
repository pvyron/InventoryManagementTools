using InMa.Torrents;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace InMa.Shopping.Components.Torrents.Pages;

public partial class SearchTorrents
{
    [Inject] private TorrentSearchService TorrentSearchService { get; set; } = null!;
    [Inject] private TorrentDownloadService TorrentDownloadService { get; set; } = null!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = null!;

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
    
    private async Task CopyTextToClipboard(Torrent torrent)
    {
        await JSRuntime.InvokeVoidAsync("navigator.clipboard.writeText", torrent.MagnetUrl);
        await JSRuntime.InvokeVoidAsync("alert", "Magnet link copied to clipboard");
    }
}