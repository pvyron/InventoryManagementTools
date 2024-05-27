namespace InMa.Shopping.DomainModels;

public sealed class ShoppingListItem : Entity
{
    public required string Product { get; set; }
    public required bool IsBought { get; set; }
}