using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.FluentUI.AspNetCore.Components;
using InMa.Shopping.Components;
using InMa.Shopping.Components.Account;
using InMa.Shopping.Data;
using InMa.Shopping.Data.Repositories.Abstractions;
using InMa.Shopping.Data.Repositories.Implementations;
using InMa.Torrents;

var builder = WebApplication.CreateBuilder(args);

GlobalSettings.Load(builder.Configuration);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddFluentUIComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("AuthDb") ??
                       throw new InvalidOperationException("Connection string 'AuthDb' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
builder.Services.AddKeyedSingleton<IShoppingListsRepository>("Open", 
    (provider, _) => 
        new ShoppingListsServerRepository(
            provider.GetRequiredService<ILogger<ShoppingListsServerRepository>>(), 
            builder.Configuration.GetConnectionString("StorageAccount"),
            builder.Configuration.GetValue<string>("ShoppingLists:OpenListsTable")));
builder.Services.AddKeyedSingleton<IShoppingListsRepository>("Completed", 
    (provider, _) => 
        new ShoppingListsServerRepository(
            provider.GetRequiredService<ILogger<ShoppingListsServerRepository>>(), 
            builder.Configuration.GetConnectionString("StorageAccount"),
            builder.Configuration.GetValue<string>("ShoppingLists:CompletedListsTable")));

builder.Services.AddTorrents();

var app = builder.Build();

await RunStartupSequence(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.MapPost("/api/url/short", async context =>
{
    context.Response.StatusCode = 208;
});

app.Run();


async Task RunStartupSequence(WebApplication application)
{
    using var scope = application.Services.CreateScope();
    string[] listsRepositoryNames = ["Open", "Completed"];

    foreach (var name in listsRepositoryNames)
    {
        await scope.ServiceProvider.GetRequiredKeyedService<IShoppingListsRepository>(name).Initialize();
    }

    await scope.ServiceProvider.GetRequiredService<TorrentDownloadService>().Initialize();
}