using Microsoft.AspNetCore.Components;

namespace InMa.ShoppingList.Components.Pages;

public partial class List
{
    [Parameter] public string? ListId { get; set; }
}