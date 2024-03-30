namespace InMa.Shopping.Data.Repositories.Models;

public sealed record SaveShoppingListData
{
    public required string Name { get; init; }
    public required List<(string Product, bool? Bought)> Items { get; init; }  
};