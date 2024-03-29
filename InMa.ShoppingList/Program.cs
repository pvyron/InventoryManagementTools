using Blazored.LocalStorage;
using Microsoft.FluentUI.AspNetCore.Components;
using InMa.ShoppingList.Components;
using InMa.ShoppingList.Components.Services;
using InMa.ShoppingList.DataAccess.Repositories.Abstractions;
using InMa.ShoppingList.DataAccess.Repositories.Implementations;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddLogging();
builder.Services.AddHttpClient();
builder.Services.AddFluentUIComponents();

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<KeyCheckingService>();
builder.Services.AddScoped<IListsRepository, ListsServerRepository>();

var app = builder.Build();

await RunStartupSequence(app);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
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

app.Run();


async Task RunStartupSequence(WebApplication application)
{
    using var scope = application.Services.CreateScope();
    var listsRepository = scope.ServiceProvider.GetRequiredService<IListsRepository>();
    await listsRepository.Initialize();
}