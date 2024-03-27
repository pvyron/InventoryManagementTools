using Azure.Data.Tables;
using InMa.ShoppingList.DataAccess.Models;
using InMa.ShoppingList.DomainModels;

namespace InMa.ShoppingList.DataAccess.Repositories;

public sealed class ListsServerRepository : IListsRepository
{
    private readonly ILogger<ListsServerRepository> _logger;
    private readonly TableClient _tableClient;
    
    public ListsServerRepository(IConfiguration configuration, ILogger<ListsServerRepository> logger)
    {
        _logger = logger;
        
        // TODO: make sure this exists on app startup
        _tableClient = new(configuration.GetConnectionString("StorageAccount"), configuration.GetValue<string>("ShoppingLists:ListsTable"));
    }

    public ValueTask<List> UpdateShoppingList(string userId, string listId,
        List<(string Product, bool? Bought)> items, CancellationToken cancellationToken)
    {
        return UpdateShoppingList(userId, EntityId.Existing(listId), items, cancellationToken);
    }
    
    public ValueTask<List> SaveShoppingList(string userId, List<(string Product, bool? Bought)> items, CancellationToken cancellationToken)
    {
        return UpdateShoppingList(userId, EntityId.New(), items, cancellationToken);
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

    private async ValueTask<List> UpdateShoppingList(string userId, EntityId listId,
        List<(string Product, bool? Bought)> items, CancellationToken cancellationToken)
    {
        List list = new()
        {
            Id = listId,
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
}