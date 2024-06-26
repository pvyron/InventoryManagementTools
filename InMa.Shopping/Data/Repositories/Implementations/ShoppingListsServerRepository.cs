﻿using Azure.Data.Tables;
using InMa.Shopping.Data.Models;
using InMa.Shopping.Data.Repositories.Abstractions;
using InMa.Shopping.Data.Repositories.Models;
using InMa.Shopping.DomainModels;

namespace InMa.Shopping.Data.Repositories.Implementations;

public sealed class ShoppingListsServerRepository : IShoppingListsRepository
{
    private readonly ILogger<ShoppingListsServerRepository> _logger;
    private readonly TableClient _tableClient;
    
    public ShoppingListsServerRepository(ILogger<ShoppingListsServerRepository> logger, string? connectionString, string? tableName)
    {
        _logger = logger;

        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString, nameof(connectionString));
        ArgumentException.ThrowIfNullOrWhiteSpace(tableName, nameof(tableName));
        
        _tableClient = new(connectionString, tableName);
    }

    public async ValueTask<ShoppingList> UpdateShoppingList(string userId, UpdateShoppingListData updateData, CancellationToken cancellationToken)
    {
        ShoppingList shoppingList = new()
        {
            Id = EntityId.Existing(updateData.Id),
            Name = updateData.Name,
            CreatedAt = updateData.CreatedAt,
            CompletedAt = updateData.CompletedAt,
            Items = updateData.Items
                .Select(i => 
                    new ShoppingListItem
                    {
                        Id = EntityId.New(), 
                        Product = i.Product, 
                        IsBought = i.IsBought
                    })
                .ToList()
        };

        try
        {
            // TODO: create mapper
            ListTableEntity entity = new()
            {
                PartitionKey = userId,
                RowKey = shoppingList.Id,
                Name = shoppingList.Name,
                CreatedAt = shoppingList.CreatedAt,
                CompletedAt = shoppingList.CompletedAt
            };
            entity.SetItems(shoppingList.Items.Select(i => (i.Product, i.IsBought)));
            
            await _tableClient.UpsertEntityAsync(entity, mode: TableUpdateMode.Replace,
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                "Failed to save shopping list with id: {shoppingListId} for user: {userId}, with items: {items}. Error: {errorMessage}", 
                shoppingList.Id,
                userId, 
                string.Join(", ", updateData.Items.Select(i => $"{i.Product} - {i.IsBought}")), 
                ex.Message);
        }
        
        return shoppingList;
    }
    
    public ValueTask<ShoppingList> SaveShoppingList(string userId, SaveShoppingListData saveData, CancellationToken cancellationToken)
    {
        var updateData = new UpdateShoppingListData
        {
            Id = EntityId.New(),
            Name = saveData.Name,
            CreatedAt = saveData.CreatedAt,
            CompletedAt = saveData.CompletedAt,
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

    public async ValueTask<ShoppingList?> GetShoppingList(string userId, string listId, CancellationToken cancellationToken)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(userId);
            ArgumentException.ThrowIfNullOrWhiteSpace(listId);
            
            var response =
                await _tableClient.GetEntityAsync<ListTableEntity>(userId, listId, cancellationToken: cancellationToken);

            if (!response.HasValue) return null;

            var entity = response.Value;

            return new ShoppingList()
            {
                Id = EntityId.Existing(entity.RowKey),
                Name = entity.Name,
                CreatedAt = entity.CreatedAt.GetValueOrDefault(entity.Timestamp.GetValueOrDefault(DateTimeOffset.UtcNow)), // TODO remove this after ensuring data cleansing
                CompletedAt = entity.CompletedAt,
                Items = entity
                    .GetItems()
                    .Select(i => 
                        new ShoppingListItem
                        {
                            Id = EntityId.New(), 
                            Product = i.Product, 
                            IsBought = i.IsBought
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

    public ValueTask<IEnumerable<ShoppingList>> GetShoppingListsForUser(string userId, CancellationToken cancellationToken)
    {
        return GetShoppingListsForUser(userId, false, null, cancellationToken);
    }

    public async ValueTask<IEnumerable<ShoppingList>> GetShoppingListsForUser(string userId, bool ordered, int? limit, CancellationToken cancellationToken)
    {
        try
        {
            var lists = new List<ShoppingList>();

            var queryText = $"PartitionKey eq '{userId}'";
            
            var listPages = _tableClient.QueryAsync<ListTableEntity>(queryText, cancellationToken: cancellationToken).AsPages();

            await foreach (var listPage in listPages)
            {
                foreach (var listTableEntity in listPage.Values)
                {
                    lists.Add(new ShoppingList
                    {
                        Id = EntityId.Existing(listTableEntity.RowKey),
                        Name = listTableEntity.Name,
                        CreatedAt = listTableEntity.CreatedAt.GetValueOrDefault(listTableEntity.Timestamp.GetValueOrDefault(DateTimeOffset.UtcNow)), // TODO remove this after ensuring data cleansing
                        CompletedAt = listTableEntity.CompletedAt,
                        Items = listTableEntity
                            .GetItems()
                            .Select(i => 
                                new ShoppingListItem
                                {
                                    Id = EntityId.New(), 
                                    Product = i.Product, 
                                    IsBought = i.IsBought
                                })
                            .ToList()
                    });
                }
            }
            
            if (!ordered && limit is null)
                return lists;
                
            if (ordered && limit is not null)
                return lists.OrderByDescending(l => l.CreatedAt).Take((int)limit);

            if (limit is null)
                return lists.OrderByDescending(l => l.CreatedAt);

            return lists.Take((int)limit);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                "Failed to fetch shopping lists for user: {userId}. Error: {errorMessage}",
                userId, 
                ex.Message);

            return Enumerable.Empty<ShoppingList>();
        }
    }

    public async Task Initialize()
    {
        await _tableClient.CreateIfNotExistsAsync();
    }
}