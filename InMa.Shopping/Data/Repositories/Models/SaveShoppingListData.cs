namespace InMa.Shopping.Data.Repositories.Models;

public sealed record SaveShoppingListData
{
    public required string Name { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required DateTimeOffset? CompletedAt { get; init; }
    public required List<(string Product, bool? Bought)> Items { get; init; }  
};