using Microsoft.AspNetCore.Components;

namespace InMa.Shopping.Components.ShoppingLists.Pages;

public partial class ExistingList
{
    [Parameter] public string? ListId { get; set; }
}