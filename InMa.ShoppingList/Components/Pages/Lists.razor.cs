using InMa.ShoppingList.DataAccess.Repositories;
using InMa.ShoppingList.DomainExtensions;
using Microsoft.AspNetCore.Components;

namespace InMa.ShoppingList.Components.Pages;

public partial class Lists
{
    [Inject] private IListsRepository listsRepository { get; set; } = null!;
    [Inject] private NavigationManager navigationManager { get; set; } = null!;

    private List<DomainModels.List> lists { get; set; } = new();
    private DomainModels.List? selectedList { get; set; }

    protected override async Task OnInitializedAsync()
    {
        lists = await listsRepository.GetShoppingListsForUser("test-user", CancellationToken.None).ToListAsync();
        
        await base.OnInitializedAsync();
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
       
        navigationManager.NavigateTo($"/lists/saved/{selectedList.Id}");
    }

    void GoToNewList()
    {
        navigationManager.NavigateTo($"/lists/new");
    }
}