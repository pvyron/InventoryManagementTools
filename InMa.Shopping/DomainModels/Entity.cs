namespace InMa.Shopping.DomainModels;

public abstract record Entity
{
    public required EntityId Id { get; init; }
}