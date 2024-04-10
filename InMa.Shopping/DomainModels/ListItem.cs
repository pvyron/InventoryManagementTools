namespace InMa.Shopping.DomainModels;

public sealed class ListItem : Entity
{
    public required string Product { get; set; }
    public required ListItemBoughtStatus Status { get; set; }
}

public enum ListItemBoughtStatus 
{
    None,
    Bought,
    NotBought
}