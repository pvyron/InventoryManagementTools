using InMa.Shopping.Data.Repositories.Abstractions;
using InMa.Shopping.DomainExtensions;
using InMa.Shopping.DomainModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace InMa.Shopping.Components.Pages;

public partial class Home
{
    [Inject(Key = "Open")] public IListsRepository OpenListsRepository { get; set; } = null!;
    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;

    private string? _username;
    private List<ShoppingList> ShoppingLists { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        _username ??= (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User.Identity?.Name;

        ShoppingLists = await OpenListsRepository.GetShoppingListsForUser(await GetUsername(), true, 3, CancellationToken.None).ToListAsync();
    }
    
    async Task<string> GetUsername()
    {
        return _username?? "invalid-user";
    }
}