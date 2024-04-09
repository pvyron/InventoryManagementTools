namespace InMa.Shopping.ViewModels;

public sealed class ListViewModel
{
    public string? ListId { get; set; }
    public string ListName { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
    public DateTimeOffset? CompletedAt { get; set; }
    public List<ListItem> Items { get; set; } = new();
}