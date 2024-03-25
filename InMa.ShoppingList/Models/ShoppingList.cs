namespace InMa.ShoppingList.Models;

public sealed record ShoppingList
{
    public required ShoppingListId Id { get; set; }
    public required List<(string Product, bool? Bought)> Items { get; set; }
}

public record struct ShoppingListId
{
    private ShoppingListId(Guid id)
    {
        Id = id.ToString("N");
    }
    
    private ShoppingListId(string id)
    {
        Id = id;
    }

    public string Id { get; } = null!;

    public static ShoppingListId New() => new(Guid.NewGuid());
    public static ShoppingListId Existing(string id) => new(id);
    
    public static implicit operator string(ShoppingListId id) => id.Id;
}