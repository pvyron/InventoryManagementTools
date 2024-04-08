using System.Timers;
using InMa.Shopping.Components.Account;
using InMa.Shopping.Components.Layout;
using InMa.Shopping.Data;
using InMa.Shopping.Data.Repositories.Abstractions;
using InMa.Shopping.Data.Repositories.Models;
using InMa.Shopping.DomainExtensions;
using InMa.Shopping.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace InMa.Shopping.Components.ShoppingLists.Shared;

public partial class ShoppingList
{
    [Inject(Key = "Open")] private IListsRepository OpenListsRepository { get; set; } = null!;
    [Inject(Key = "Completed")] private IListsRepository CompletedListsRepository { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;
    [Parameter] public string? ListId { get; set; }
    [Parameter] public ShoppingListStateEnum ListState { get; set; }

    private string? _username;
    private System.Timers.Timer _savingCooldown = new(10000)
    {
        AutoReset = true,
        Enabled = true
    };

    private bool _awaitingSave = false;

    private ListViewModel ListViewModel { get; set; } = new();

    private string ShoppingListTittle => ListId is null ? "Shopping List" : $"Shopping List - {ListViewModel.ListName}";
    private string NewProductName { get; set; } = string.Empty;
    private bool AddingProduct { get; set; } = false;
    private bool RemovingProduct { get; set; } = false;
    private bool SavingList { get; set; }
    private bool DeletingList { get; set; } = false;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        _username ??= (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User.Identity?.Name;

        if (ListId is null)
            return;

        var list = ListState switch
        {
            ShoppingListStateEnum.Completed => await CompletedListsRepository.GetShoppingList(await GetUsername(),
                ListId, CancellationToken.None),
            _ => await OpenListsRepository.GetShoppingList(await GetUsername(), ListId,
                CancellationToken.None)
        };
    
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

        _savingCooldown.Elapsed += async (sender, e) => await OnAutosaveCooldown(sender, e);
    }

    async Task SaveList(bool forced = false)
    {
        if (!forced)
        {
            Console.WriteLine("Save not forced added to queue");
            _awaitingSave = true;
            return;
        }

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

                var newList = ListState switch
                {
                    ShoppingListStateEnum.Completed =>
                        await CompletedListsRepository.SaveShoppingList(await GetUsername(), saveData,
                            CancellationToken.None),
                    _ => await OpenListsRepository.SaveShoppingList(await GetUsername(), saveData,
                            CancellationToken.None),
                };
                
                NavigationManager.NavigateTo(ListState == ShoppingListStateEnum.Completed
                    ? $"/lists/completed/{newList.Id}"
                    : $"/lists/open/{newList.Id}");
            }
            else
            {
                var updateData = new UpdateShoppingListData
                {
                    Id = ListId,
                    Name = ListViewModel.ListName,
                    Items = ListViewModel.Items.Select(i => (i.Product, i.Bought)).ToList()
                };

                var updatedList = ListState switch
                {
                    ShoppingListStateEnum.Completed => await CompletedListsRepository.UpdateShoppingList(
                        await GetUsername(), updateData, CancellationToken.None),
                    _ => await OpenListsRepository.UpdateShoppingList(await GetUsername(), updateData,
                        CancellationToken.None)
                };
            }
        }
        finally
        {
            SavingList = false;
            _awaitingSave = false;
        }
    }

    async Task CompleteList()
    {
        try
        {
            _awaitingSave = false;
            SavingList = true;

            if (string.IsNullOrWhiteSpace(ListViewModel.ListName) || ListId is null || ListState == ShoppingListStateEnum.Completed)
                return;

            var saveData = new SaveShoppingListData
            {
                Name = ListViewModel.ListName,
                Items = ListViewModel.Items.Select(i => (i.Product, i.Bought)).ToList()
            };

            var newList =
                await CompletedListsRepository.SaveShoppingList(await GetUsername(), saveData, CancellationToken.None);

            await OpenListsRepository.DeleteShoppingList(await GetUsername(), ListId, CancellationToken.None);
            
            // TODO manage transaction maybe?
            // investigate for CQRS pattern or a domain shopping service

            NavigationManager.NavigateTo($"/lists/completed/{newList.Id}");
        }
        finally
        {
            SavingList = false;
            _awaitingSave = false;
        }
    }

    async Task DeleteList()
    {
        try
        {
            DeletingList = true;

            if (ListId is null)
                return;

            switch (ListState)
            {
                case ShoppingListStateEnum.Completed:
                    await CompletedListsRepository.DeleteShoppingList(await GetUsername(),
                        ListId, CancellationToken.None);
                    break;
                case ShoppingListStateEnum.Open:
                default:
                    await OpenListsRepository.DeleteShoppingList(await GetUsername(), ListId, CancellationToken.None);
                    break;
            }

            NavigationManager.NavigateTo(ListState == ShoppingListStateEnum.Completed
                ? $"/lists/completed"
                : $"/lists/open");
        }
        finally
        {
            DeletingList = false;
        }
    }

    async Task<string> GetUsername()
    {
        return _username?? "invalid-user";
    }

    async Task OnAutosaveCooldown(object? source, ElapsedEventArgs e)
    {
        try
        {
            if (_awaitingSave) await SaveList(true);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    Task SaveListButtonClicked() => SaveList(true);

    Task CompleteListButtonClicked() => CompleteList();
    
    Task AddNewProductButtonClicked()
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

    Task RemoveProductButtonClicked(string productName)
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

    Task CheckboxChanged()
    {
        return SaveList();
    }
}