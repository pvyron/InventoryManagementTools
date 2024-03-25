namespace InMa.ShoppingList.Models;

public sealed class ShoppingListItem(string product)
{
    public bool? Bought { get; set; }
    public string Product { get; set; } = product;
}