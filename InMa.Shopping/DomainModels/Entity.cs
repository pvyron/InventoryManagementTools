namespace InMa.Shopping.DomainModels;

public abstract class Entity
{
    public required EntityId Id { get; init; }
}