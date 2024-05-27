using System.Text.Json;
using Azure;
using Azure.Data.Tables;

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

    public IEnumerable<(string Product, bool IsBought)> GetItems(ShoppingListItemsSortMethod method = ShoppingListItemsSortMethod.PendingFirst)
    {
        var boughtProducts = JsonSerializer.Deserialize<List<string>>(BoughtProducts);
        var notBoughtProducts = JsonSerializer.Deserialize<List<string>>(NotBoughtProducts);

        return method switch
        {
            ShoppingListItemsSortMethod.DoneFirst => 
                AggregateBoughtFirst(boughtProducts: boughtProducts, notBoughtProducts: notBoughtProducts),
            _ => 
                AggregatePendingFirst(boughtProducts: boughtProducts, notBoughtProducts: notBoughtProducts)
        };
    }

    private IEnumerable<(string Product, bool IsBought)> AggregateBoughtFirst(List<string>? boughtProducts,
        List<string>? notBoughtProducts)
    {
        var result = Enumerable.Empty<(string Product, bool IsBought)>();
        
        if (notBoughtProducts is not null)
            result = result.Union(notBoughtProducts.Select(p => (p, false)));

        if (boughtProducts is not null)
            result = result.Union(boughtProducts.Select(p => (p, true)));
        
        return result;
    }
    
    private IEnumerable<(string Product, bool IsBought)> AggregatePendingFirst(List<string>? boughtProducts,
        List<string>? notBoughtProducts)
    {
        var result = Enumerable.Empty<(string Product, bool IsBought)>();

        if (boughtProducts is not null)
            result = result.Union(boughtProducts.Select(p => (p, true)));
        
        if (notBoughtProducts is not null)
            result = result.Union(notBoughtProducts.Select(p => (p, false)));
        
        return result;
    }

    public void SetItems(IEnumerable<(string Product, bool IsBought)> items)
    {
        var boughtProducts = new List<string>();
        var notBoughtProducts = new List<string>();
        
        foreach (var item in items)
        {
            if (item.IsBought)
            {
                boughtProducts.Add(item.Product);
            }
            else
            {
                notBoughtProducts.Add(item.Product);
            }
        }

        BoughtProducts = JsonSerializer.Serialize(boughtProducts);
        NotBoughtProducts = JsonSerializer.Serialize(notBoughtProducts);
    }
}

public enum ShoppingListItemsSortMethod
{
    DoneFirst,
    PendingFirst,
}