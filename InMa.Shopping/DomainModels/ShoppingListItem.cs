namespace InMa.Shopping.DomainModels;

public sealed record ShoppingListItem : Entity
{
    public required string Product { get; set; }
    public required bool IsBought { get; set; }
}