namespace InMa.ShoppingList.ViewModels;

public sealed class ListViewModel
{
    public string? ListId { get; set; }
    public string ListName { get; set; } = string.Empty;
    public List<ListItem> Items { get; set; } = new();
}