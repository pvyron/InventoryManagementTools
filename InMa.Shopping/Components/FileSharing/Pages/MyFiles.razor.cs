using InMa.Shopping.Data.Repositories.Abstractions;
using InMa.Shopping.Data.Repositories.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.FluentUI.AspNetCore.Components;

namespace InMa.Shopping.Components.FileSharing.Pages;

public partial class MyFiles
{
    [Inject] private IFilesRepository FilesRepository { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;

    private string? _username;
    private SearchFileResult[] _searchFileResults { get; set; } = [];

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        _username ??= (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User.Identity?.Name;

        _searchFileResults = await FilesRepository.SearchFilesForUser(await GetUsername(), CancellationToken.None);
    }
    
    Task<string> GetUsername()
    {
        return Task.FromResult(_username?? "invalid-user");
    }
}