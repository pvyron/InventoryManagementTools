namespace InMa.ShoppingList.DomainModels;

public abstract record Entity
{
    public required EntityId Id { get; init; }
}