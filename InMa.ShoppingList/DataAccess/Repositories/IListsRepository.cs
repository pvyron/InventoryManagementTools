using InMa.ShoppingList.DomainModels;

namespace InMa.ShoppingList.DataAccess.Repositories;

public interface IListsRepository
{
    ValueTask<List> UpdateShoppingList(string userId, string listId,
        List<(string Product, bool? Bought)> items, CancellationToken cancellationToken);

    ValueTask<List> SaveShoppingList(string userId, List<(string Product, bool? Bought)> items, CancellationToken cancellationToken);
    ValueTask<List?> GetShoppingList(string userId, string listId, CancellationToken cancellationToken);
}