using InMa.Torrents;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddHttpClient("torrent-search", client =>
{
    client.BaseAddress = new Uri("https://apibay.org/q.php");
});
builder.Services.AddSingleton<TorrentSearchService>();

var app = builder.Build();

var searchService = app.Services.GetRequiredService<TorrentSearchService>();

while (true)
{
    Console.Write("Search for: ");
    var searchQuery = Console.ReadLine()?.Trim();

    if (string.IsNullOrWhiteSpace(searchQuery))
    {
        Console.WriteLine("Type exit to exit");
        continue;
    }

    if (searchQuery.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        break;
    }

    foreach (var torrent in await searchService.Search(searchQuery, CancellationToken.None))
    {
        Console.WriteLine(torrent.MagnetUrl);
    }
    
    Console.WriteLine("-------------------------");
    Console.Write("Exit? (y/n):");

    if (Console.ReadLine()?.Trim().Equals("y", StringComparison.OrdinalIgnoreCase) ?? false)
    {
        break;
    }
}