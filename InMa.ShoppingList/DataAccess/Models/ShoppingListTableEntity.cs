using Azure;
using Azure.Data.Tables;

namespace InMa.ShoppingList.DataAccess.Models;

public class ShoppingListTableEntity : ITableEntity
{
    public string PartitionKey { get; set; } = null!;
    public string RowKey { get; set; } = null!;
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    public List<(string Product, bool? Bought)>? Items { get; set; }
}