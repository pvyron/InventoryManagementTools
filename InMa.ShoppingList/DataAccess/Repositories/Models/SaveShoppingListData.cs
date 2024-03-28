namespace InMa.ShoppingList.DataAccess.Repositories.Models;

public sealed record SaveShoppingListData
{
    public required string Name { get; init; }
    public required List<(string Product, bool? Bought)> Items { get; init; }  
};