using InMa.ShoppingList.DataAccess.Repositories;
using InMa.ShoppingList.ViewModels;
using Microsoft.AspNetCore.Components;

namespace InMa.ShoppingList.Components.Pages;

public partial class Lists
{
    [Inject] private IListsRepository listsRepository { get; set; } = null!;
    [Inject] private NavigationManager navigationManager { get; set; } = null!;

    private List<ListItem> Items { get; set; } = new();

    private string NewProductName { get; set; } = string.Empty;
    private bool AddingProduct { get; set; } = false;
    private bool SavingList { get; set; } = false;

    async Task SaveList()
    {
        try
        {
            SavingList = true;

            var newList = await listsRepository.SaveShoppingList("test-user",
                Items.Select(i => (i.Product, i.Bought)).ToList(), CancellationToken.None);

            navigationManager.NavigateTo($"lists/{newList.Id}");
        }
        finally
        {
            SavingList = false;
        }
    }

    Task AddNewProduct()
    {
        try
        {
            AddingProduct = true;
            
            if (string.IsNullOrWhiteSpace(NewProductName))
            {
                return Task.CompletedTask;
            }

            if (Items.Any(i => i.Product.Equals(NewProductName, StringComparison.OrdinalIgnoreCase)))
            {
                return Task.CompletedTask;
            }

            Items.Add(new ListItem(NewProductName));
            
            return Task.CompletedTask;
        }
        finally
        {
            AddingProduct = false;
        }
    }
}