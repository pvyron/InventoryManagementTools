using Microsoft.AspNetCore.Components;

namespace InMa.Shopping.Components.Pages;

public partial class ShoppingList
{
    [Parameter] public string? ListId { get; set; }
}