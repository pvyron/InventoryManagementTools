using InMa.Torrents;
using InMa.TorrentsService;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddTorrents(() =>
{
    var password = PasswordRepository.GetPassword();

    while (string.IsNullOrWhiteSpace(password))
    {
        Console.WriteLine("Provide storage account connection string");
        
        password = Console.ReadLine();
        
        PasswordRepository.SavePassword(password ?? "");
    }
    
    return new TorrentSettings
    {
        StorageAccountConnectionString = password
    };
});
builder.Services.AddHostedService<TorrentsService>();

var host = builder.Build();
host.Run();