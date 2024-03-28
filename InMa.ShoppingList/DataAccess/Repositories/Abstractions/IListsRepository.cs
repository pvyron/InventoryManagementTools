using InMa.ShoppingList.DataAccess.Repositories.Models;
using InMa.ShoppingList.DomainModels;

namespace InMa.ShoppingList.DataAccess.Repositories;

public interface IListsRepository
{
    ValueTask<List> UpdateShoppingList(string userId, UpdateShoppingListData updateData, CancellationToken cancellationToken);

    ValueTask<List> SaveShoppingList(string userId, SaveShoppingListData saveData, CancellationToken cancellationToken);
    
    ValueTask<List?> GetShoppingList(string userId, string listId, CancellationToken cancellationToken);

    ValueTask<IEnumerable<List>> GetShoppingListsForUser(string userId, CancellationToken cancellationToken);
}