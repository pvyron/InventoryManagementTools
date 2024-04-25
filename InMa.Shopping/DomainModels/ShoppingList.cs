namespace InMa.Shopping.DomainModels;

public sealed record ShoppingList : Entity
{
    public required List<ShoppingListItem> Items { get; init; }
    public required string Name { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public required DateTimeOffset? CompletedAt { get; set; }
}