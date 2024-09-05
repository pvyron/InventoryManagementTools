using InMa.Torrents;
using InMa.TorrentsService;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddTorrents();
builder.Services.AddHostedService<TorrentsService>();

var host = builder.Build();
host.Run();