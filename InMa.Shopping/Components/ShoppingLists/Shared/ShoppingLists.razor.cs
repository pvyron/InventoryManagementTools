using InMa.Shopping.Data.Repositories.Abstractions;
using InMa.Shopping.DomainExtensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace InMa.Shopping.Components.ShoppingLists.Shared;

public partial class ShoppingLists
{
    [Inject] private IListsRepository listsRepository { get; set; } = null!;
    [Inject] private NavigationManager navigationManager { get; set; } = null!;
    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;
    [Parameter] public ShoppingListStateEnum ListState { get; set; }

    private string? _username;
    private List<DomainModels.List> lists { get; set; } = new();
    private DomainModels.List? selectedList { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        _username ??= (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User.Identity?.Name;
        
        lists = await listsRepository.GetOpenShoppingListsForUser(await GetUsername(), CancellationToken.None).ToListAsync();
    }

    void SelectedListChanged(string? pickedListId)
    {
        if (string.IsNullOrWhiteSpace(pickedListId))
            return;

        selectedList = lists.FirstOrDefault(l => l.Id == pickedListId);
    }

    void GoToSelectedList()
    {
        if (selectedList is null)
            return;
        navigationManager.NavigateTo(ListState == ShoppingListStateEnum.Open
            ? $"/lists/open/{selectedList.Id}"
            : $"/lists/completed/{selectedList.Id}");
    }

    void GoToNewList()
    {
        navigationManager.NavigateTo($"/lists/new", true);
    }
    
    async Task<string> GetUsername()
    {
        return _username?? "invalid-user";
    }
}