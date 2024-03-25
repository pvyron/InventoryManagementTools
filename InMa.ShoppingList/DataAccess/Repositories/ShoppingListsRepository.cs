using Azure.Data.Tables;
using InMa.ShoppingList.DataAccess.Models;
using InMa.ShoppingList.Models;

namespace InMa.ShoppingList.DataAccess.Repositories;

public sealed class ShoppingListsRepository
{
    private readonly ILogger<ShoppingListsRepository> _logger;
    private readonly TableClient _tableClient;
    
    public ShoppingListsRepository(IConfiguration configuration, ILogger<ShoppingListsRepository> logger)
    {
        _logger = logger;
        _tableClient = new(configuration.GetConnectionString("StorageAccount"), configuration.GetValue<string>("ShoppingLists:ListsTable"));
    }

    public async ValueTask<string> SaveShoppingList(string userId, List<(string Product, bool? Bought)> items, CancellationToken cancellationToken)
    {
        ShoppingList.Models.ShoppingList shoppingList = new()
        {
            Id = ShoppingListId.New(),
            Items = items
        };

        try
        {
            ShoppingListTableEntity entity = new()
            {
                PartitionKey = userId,
                RowKey = shoppingList.Id,
                Items = shoppingList.Items
            };

            await _tableClient.UpsertEntityAsync(entity, mode: TableUpdateMode.Replace, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                "Failed to save shopping list with id: {shoppingListId} for user: {userId}, with items: {items}. Error: {errorMessage}", 
                shoppingList.Id,
                userId, 
                string.Join(", ", items.Select(i => $"{i.Product} - {i.Bought.GetValueOrDefault(false)}")), 
                ex.Message);
        }
        
        return shoppingList.Id;
    }
}