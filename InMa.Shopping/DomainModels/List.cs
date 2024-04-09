namespace InMa.Shopping.DomainModels;

public sealed record List : Entity
{
    public required List<ListItem> Items { get; init; }
    public required string Name { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public required DateTimeOffset? CompletedAt { get; set; }
}