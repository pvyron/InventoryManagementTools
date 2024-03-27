using Azure.Data.Tables;
using InMa.ShoppingList.DataAccess.Models;
using InMa.ShoppingList.DomainModels;

namespace InMa.ShoppingList.DataAccess.Repositories;

public sealed class ListsRepository
{
    private readonly ILogger<ListsRepository> _logger;
    private readonly TableClient _tableClient;
    
    public ListsRepository(IConfiguration configuration, ILogger<ListsRepository> logger)
    {
        _logger = logger;
        
        // TODO: make sure this exists on app startup
        _tableClient = new(configuration.GetConnectionString("StorageAccount"), configuration.GetValue<string>("ShoppingLists:ListsTable"));
    }

    public async ValueTask<List> SaveShoppingList(string userId, List<(string Product, bool? Bought)> items, CancellationToken cancellationToken)
    {
        List list = new()
        {
            Id = EntityId.New(),
            Items = items.Select(i => new ListItem{Id = EntityId.New(), Product = i.Product, Status = i.Bought switch
            {
                null => ListItemBoughtStatus.None,
                true => ListItemBoughtStatus.Bought,
                false => ListItemBoughtStatus.NotBought
            }}).ToList()
        };

        try
        {
            // TODO: create mapper
            ListTableEntity entity = new()
            {
                PartitionKey = userId,
                RowKey = list.Id
            };
            entity.SetItems(list.Items.Select(i => (i.Product, i.Status)));
            
            await _tableClient.UpsertEntityAsync(entity, mode: TableUpdateMode.Replace,
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                "Failed to save shopping list with id: {shoppingListId} for user: {userId}, with items: {items}. Error: {errorMessage}", 
                list.Id,
                userId, 
                string.Join(", ", items.Select(i => $"{i.Product} - {i.Bought.GetValueOrDefault(false)}")), 
                ex.Message);
        }
        
        return list;
    }

    public async ValueTask<List?> GetShoppingList(string userId, string listId, CancellationToken cancellationToken)
    {
        try
        {
            var response =
                await _tableClient.GetEntityAsync<ListTableEntity>(userId, listId, cancellationToken: cancellationToken);

            if (!response.HasValue) return null;

            var entity = response.Value;

            return new List()
            {
                Id = EntityId.Existing(entity.RowKey),
                Items = entity.GetItems().Select(i => new ListItem{Id = EntityId.New(), Product = i.Product, Status = i.Status}).ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                "Failed to fetch shopping list with id: {shoppingListId} for user: {userId}. Error: {errorMessage}", 
                listId,
                userId, 
                ex.Message);
            
            return null;
        }
    }
}