using InMa.ShoppingList.DataAccess.Repositories;
using InMa.ShoppingList.DataAccess.Repositories.Abstractions;
using InMa.ShoppingList.DataAccess.Repositories.Models;
using InMa.ShoppingList.DomainExtensions;
using InMa.ShoppingList.ViewModels;
using Microsoft.AspNetCore.Components;

namespace InMa.ShoppingList.Components.Partials;

public partial class ListPartial
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
    private bool SavingList { get; set; } = false;

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
            
            return Task.CompletedTask;
        }
        finally
        {
            AddingProduct = false;
        }
    }
}