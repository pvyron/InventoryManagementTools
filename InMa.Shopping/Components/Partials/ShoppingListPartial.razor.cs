using InMa.Shopping.Data.Repositories.Abstractions;
using InMa.Shopping.Data.Repositories.Models;
using InMa.Shopping.DomainExtensions;
using InMa.Shopping.ViewModels;
using Microsoft.AspNetCore.Components;

namespace InMa.Shopping.Components.Partials;

public partial class ShoppingListPartial
{
    [Inject] private IListsRepository ListsRepository { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Parameter] public string? ListId { get; set; }

    private ListViewModel ListViewModel { get; set; } = new();

    private string ShoppingListTittle
    {
        get
        {
            if (ListId is null)
                return "Shopping List";

            return $"Shopping List - {ListViewModel.ListName}";
        }
    }

    private string NewProductName { get; set; } = string.Empty;
    private bool AddingProduct { get; set; } = false;
    private bool RemovingProduct { get; set; } = false;
    private bool SavingList { get; set; } = false;
    private bool DeletingList { get; set; } = false;

    protected override async Task OnInitializedAsync()
    {
        if (ListId is null)
            return;

        var list = await ListsRepository.GetShoppingList("test-user", ListId, CancellationToken.None);

        if (list is null)
        {
            NavigationManager.NavigateTo("/error");
            return;
        }

        ListViewModel = new()
        {
            ListId = list.Id,
            ListName = list.Name,
            Items = list.Items
                .Select(i =>
                    new ListItem(i.Product)
                    {
                        Bought = i.Status.ToNullableBool()
                    })
                .ToList()
        };

        await base.OnInitializedAsync();
    }

    async Task SaveList()
    {
        try
        {
            SavingList = true;

            if (string.IsNullOrWhiteSpace(ListViewModel.ListName))
                return;
            
            if (ListId is null)
            {
                var saveData = new SaveShoppingListData
                {
                    Name = ListViewModel.ListName,
                    Items = ListViewModel.Items.Select(i => (i.Product, i.Bought)).ToList()
                };

                var newList = await ListsRepository.SaveShoppingList("test-user", saveData, CancellationToken.None);
                
                NavigationManager.NavigateTo($"/lists/saved/{newList.Id}");
            }
            else
            {
                var updateData = new UpdateShoppingListData
                {
                    Id = ListId,
                    Name = ListViewModel.ListName,
                    Items = ListViewModel.Items.Select(i => (i.Product, i.Bought)).ToList()
                };

                var updatedList =
                    await ListsRepository.UpdateShoppingList("test-user", updateData, CancellationToken.None);
            }
        }
        finally
        {
            SavingList = false;
        }
    }
    
    async Task DeleteList()
    {
        try
        {
            DeletingList = true;

            if (ListId is null)
                return;

            await ListsRepository.DeleteShoppingList("test-user", ListId, CancellationToken.None);
            
            NavigationManager.NavigateTo("/lists");
        }
        finally
        {
            DeletingList = false;
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

            if (ListViewModel.Items.Any(i => i.Product.Equals(NewProductName, StringComparison.OrdinalIgnoreCase)))
            {
                return Task.CompletedTask;
            }

            ListViewModel.Items.Add(new ListItem(NewProductName));
            
            return SaveList();
        }
        finally
        {
            NewProductName = string.Empty;
            AddingProduct = false;
        }
    }
    
    Task RemoveProduct(string productName)
    {
        try
        {
            RemovingProduct = true;
            
            if (string.IsNullOrWhiteSpace(productName))
            {
                return Task.CompletedTask;
            }

            if (!ListViewModel.Items.Any(i => i.Product.Equals(productName, StringComparison.OrdinalIgnoreCase)))
            {
                return Task.CompletedTask;
            }

            ListViewModel.Items.RemoveAll(i => i.Product.Equals(productName, StringComparison.OrdinalIgnoreCase));
            
            return SaveList();
        }
        finally
        {
            RemovingProduct = false;
        }
    }
}