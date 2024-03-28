using Azure.Data.Tables;
using InMa.ShoppingList.Components.Services;
using InMa.ShoppingList.DataAccess.Models;
using InMa.ShoppingList.DataAccess.Repositories.Abstractions;
using InMa.ShoppingList.DataAccess.Repositories.Models;
using InMa.ShoppingList.DomainExtensions;
using InMa.ShoppingList.DomainModels;

namespace InMa.ShoppingList.DataAccess.Repositories.Implementations;

public sealed class ListsServerRepository : IListsRepository
{
    private readonly ILogger<ListsServerRepository> _logger;
    private readonly KeyBearingService _keyBearingService;
    private readonly TableClient _tableClient;
    
    public ListsServerRepository(IConfiguration configuration, ILogger<ListsServerRepository> logger, KeyBearingService keyBearingService)
    {
        _logger = logger;
        _keyBearingService = keyBearingService;

        _tableClient = new(configuration.GetConnectionString("StorageAccount"), configuration.GetValue<string>("ShoppingLists:ListsTable"));
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

        if (!_keyBearingService.IsAuthorized()) return list;

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

    public async ValueTask<List?> GetShoppingList(string userId, string listId, CancellationToken cancellationToken)
    {
        try
        {
            if (!_keyBearingService.IsAuthorized()) return null;
            
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
            
            if (!_keyBearingService.IsAuthorized()) return lists;
            
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