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
    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;

    private SearchFileResult[] _searchFileResults { get; set; } = [];

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthenticationStateProvider.GetAuthenticationStateAsync();

        _searchFileResults =
            await FilesRepository.SearchFilesForUser(state.User.Identity!.Name!, CancellationToken.None);
        
        await base.OnInitializedAsync();
    }
}