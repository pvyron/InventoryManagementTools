using System.Text.Json;
using Azure;
using Azure.Data.Tables;
using InMa.Shopping.DomainModels;

namespace InMa.Shopping.Data.Models;

public class ListTableEntity : ITableEntity
{
    public required string PartitionKey { get; set; }
    public required string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    public required string Name { get; set; }
    public required DateTimeOffset? CreatedAt { get; set; }
    public required DateTimeOffset? CompletedAt { get; set; }
    
    public string BoughtProducts { get; set; } = string.Empty;
    public string NotBoughtProducts { get; set; } = string.Empty;
    public string UnknownStatusProducts { get; set; } = string.Empty;

    public IEnumerable<(string Product, ListItemBoughtStatus Status)> GetItems()
    {
        var boughtProducts = JsonSerializer.Deserialize<List<string>>(BoughtProducts);
        var notBoughtProducts = JsonSerializer.Deserialize<List<string>>(NotBoughtProducts);
        var unknownStatusProducts = JsonSerializer.Deserialize<List<string>>(UnknownStatusProducts);

        var result = Enumerable.Empty<(string Product, ListItemBoughtStatus Status)>();

        if (boughtProducts is not null)
            result = result.Union(boughtProducts.Select(p => (p, ListItemBoughtStatus.Bought)));

        if (notBoughtProducts is not null)
            result = result.Union(notBoughtProducts.Select(p => (p, ListItemBoughtStatus.NotBought)));
        
        if (unknownStatusProducts is not null)
            result = result.Union(unknownStatusProducts.Select(p => (p, ListItemBoughtStatus.None)));
        
        return result;
    }

    public void SetItems(IEnumerable<(string Product, ListItemBoughtStatus Status)> items)
    {
        var boughtProducts = new List<string>();
        var notBoughtProducts = new List<string>();
        var unknownStatusProducts = new List<string>();
        
        foreach (var item in items)
        {
            switch (item.Status)
            {
                case ListItemBoughtStatus.Bought:
                    boughtProducts.Add(item.Product);
                    break;
                case ListItemBoughtStatus.NotBought:
                    notBoughtProducts.Add(item.Product);
                    break;
                case ListItemBoughtStatus.None:
                    unknownStatusProducts.Add(item.Product);
                    break;
            }
        }

        BoughtProducts = JsonSerializer.Serialize(boughtProducts);
        NotBoughtProducts = JsonSerializer.Serialize(notBoughtProducts);
        UnknownStatusProducts = JsonSerializer.Serialize(unknownStatusProducts);

    }
}