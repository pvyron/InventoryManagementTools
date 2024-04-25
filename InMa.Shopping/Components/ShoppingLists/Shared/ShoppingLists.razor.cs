using InMa.Shopping.Data.Repositories.Abstractions;
using InMa.Shopping.DomainExtensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace InMa.Shopping.Components.ShoppingLists.Shared;

public partial class ShoppingLists
{
    [Inject(Key = "Open")] private IListsRepository OpenListsRepository { get; set; } = null!;
    [Inject(Key = "Completed")] private IListsRepository CompletedListsRepository { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;
    [Parameter] public ShoppingListStateEnum ListState { get; set; }

    private string? _username;
    private List<DomainModels.ShoppingList> lists { get; set; } = new();
    private DomainModels.ShoppingList? selectedList { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        _username ??= (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User.Identity?.Name;

        lists = ListState switch
        {
            ShoppingListStateEnum.Completed => await CompletedListsRepository
                .GetShoppingListsForUser(await GetUsername(), CancellationToken.None).ToListAsync(),
            _ => await OpenListsRepository.GetShoppingListsForUser(await GetUsername(), CancellationToken.None)
                .ToListAsync()
        };
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
        NavigationManager.NavigateTo(ListState == ShoppingListStateEnum.Completed
            ? $"/lists/completed/{selectedList.Id}"
            : $"/lists/open/{selectedList.Id}");
    }

    void GoToNewList()
    {
        NavigationManager.NavigateTo($"/lists/new", true);
    }
    
    async Task<string> GetUsername()
    {
        return _username?? "invalid-user";
    }
}