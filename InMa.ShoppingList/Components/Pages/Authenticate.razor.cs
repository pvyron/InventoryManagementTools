using InMa.ShoppingList.Components.Services;
using Microsoft.AspNetCore.Components;

namespace InMa.ShoppingList.Components.Pages;

public partial class Authenticate
{
    [Inject] private KeyCheckingService KeyBearingService { get; set; } = null!;
    
    private string Key { get; set; } = string.Empty;

    async Task SaveKey()
    {
        await KeyBearingService.SaveKey(Key, CancellationToken.None);
    }
}