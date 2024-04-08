using Azure.Data.Tables;
using InMa.Shopping.Data.Models;
using InMa.Shopping.Data.Repositories.Abstractions;
using InMa.Shopping.Data.Repositories.Models;
using InMa.Shopping.DomainExtensions;
using InMa.Shopping.DomainModels;

namespace InMa.Shopping.Data.Repositories.Implementations;

public sealed class ListsServerRepository : IListsRepository
{
    private readonly ILogger<ListsServerRepository> _logger;
    private readonly TableClient _tableClient;
    
    public ListsServerRepository(ILogger<ListsServerRepository> logger, string? connectionString, string? tableName)
    {
        _logger = logger;

        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString, nameof(connectionString));
        ArgumentException.ThrowIfNullOrWhiteSpace(tableName, nameof(tableName));
        
        _tableClient = new(connectionString, tableName);
    }

    public async ValueTask<List> UpdateShoppingList(string userId, UpdateShoppingListData updateData, CancellationToken cancellationToken)
    {
        List list = new()
        {
            Id = EntityId.Existing(updateData.Id),
            Name = updateData.Name,
            Items = updateData.Items
                .Select(i => 
                    new ListItem
                    {
                        Id = EntityId.New(), 
                        Product = i.Product, 
                        Status = i.Bought.ToListItemBoughtStatus()
                    })
                .ToList()
        };

        try
        {
            // TODO: create mapper
            ListTableEntity entity = new()
            {
                PartitionKey = userId,
                RowKey = list.Id,
                Name = list.Name
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
                string.Join(", ", updateData.Items.Select(i => $"{i.Product} - {i.Bought.GetValueOrDefault(false)}")), 
                ex.Message);
        }
        
        return list;
    }
    
    public ValueTask<List> SaveShoppingList(string userId, SaveShoppingListData saveData, CancellationToken cancellationToken)
    {
        var updateData = new UpdateShoppingListData
        {
            Id = EntityId.New(),
            Name = saveData.Name,
            Items = saveData.Items
        };
        
        return UpdateShoppingList(userId, updateData, cancellationToken);
    }

    public async ValueTask DeleteShoppingList(string userId, string listId, CancellationToken cancellationToken)
    {
        try
        {
            await _tableClient.DeleteEntityAsync(userId, listId, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                "Failed to delete shopping list with id: {listId} for user: {userId}. Error: {errorMessage}", 
                listId,
                userId, 
                ex.Message);
        }
    }

    public async ValueTask<List?> GetShoppingList(string userId, string listId, CancellationToken cancellationToken)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(userId);
            ArgumentException.ThrowIfNullOrWhiteSpace(listId);
            
            var response =
                await _tableClient.GetEntityAsync<ListTableEntity>(userId, listId, cancellationToken: cancellationToken);

            if (!response.HasValue) return null;

            var entity = response.Value;

            return new List()
            {
                Id = EntityId.Existing(entity.RowKey),
                Name = entity.Name,
                Items = entity
                    .GetItems()
                    .Select(i => 
                        new ListItem
                        {
                            Id = EntityId.New(), 
                            Product = i.Product, 
                            Status = i.Status
                        })
                    .ToList()
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

    public async ValueTask<IEnumerable<List>> GetShoppingListsForUser(string userId, CancellationToken cancellationToken)
    {
        try
        {
            var lists = new List<List>();
            
            var listPages = _tableClient.QueryAsync<ListTableEntity>($"PartitionKey eq '{userId}'").AsPages();

            await foreach (var listPage in listPages)
            {
                foreach (var listTableEntity in listPage.Values)
                {
                    lists.Add(new List
                    {
                        Id = EntityId.Existing(listTableEntity.RowKey),
                        Name = listTableEntity.Name,
                        Items = listTableEntity
                            .GetItems()
                            .Select(i => 
                                new ListItem
                                {
                                    Id = EntityId.New(), 
                                    Product = i.Product, 
                                    Status = i.Status
                                })
                            .ToList()
                    });
                }
            }

            return lists;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                "Failed to fetch shopping lists for user: {userId}. Error: {errorMessage}",
                userId, 
                ex.Message);

            return Enumerable.Empty<List>();
        }
    }

    public async Task Initialize()
    {
        await _tableClient.CreateIfNotExistsAsync();
    }
}