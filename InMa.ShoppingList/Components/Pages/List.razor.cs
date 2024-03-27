using InMa.ShoppingList.DataAccess.Repositories;
using InMa.ShoppingList.DomainExtensions;
using InMa.ShoppingList.DomainModels;
using Microsoft.AspNetCore.Components;
using ListItem = InMa.ShoppingList.ViewModels.ListItem;

namespace InMa.ShoppingList.Components.Pages;

public partial class List
{
    [Inject] private IListsRepository ListsRepository { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Parameter] public string ListId { get; set; } = null!;
 
    private List<ListItem> Items { get; set; } = new();

    private string NewProductName { get; set; } = string.Empty;
    private bool AddingProduct { get; set; } = false;
    private bool SavingList { get; set; } = false;

    protected override async Task OnInitializedAsync()
    {
        var list = await ListsRepository.GetShoppingList("test-user", ListId, CancellationToken.None);

        if (list is null)
        {
            NavigationManager.NavigateTo("/error");
            return;
        }

        Items = list.Items
            .Select(i =>
                new ListItem(i.Product)
                {
                    Bought = i.Status.ToNullableBool()
                })
            .ToList();

        await base.OnInitializedAsync();
    }

    async Task SaveList()
    {
        try
        {
            SavingList = true;

            var updatedList = await ListsRepository.UpdateShoppingList("test-user", ListId,
                Items.Select(i => (i.Product, i.Bought)).ToList(), CancellationToken.None);
            
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