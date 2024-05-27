using InMa.Shopping.Data.Repositories.Models;
using InMa.Shopping.DomainModels;

namespace InMa.Shopping.Data.Repositories.Abstractions;

public interface IShoppingListsRepository : IStartupProcess
{
    ValueTask<ShoppingList> UpdateShoppingList(string userId, UpdateShoppingListData updateData, CancellationToken cancellationToken);

    ValueTask<ShoppingList> SaveShoppingList(string userId, SaveShoppingListData saveData, CancellationToken cancellationToken);

    ValueTask DeleteShoppingList(string userId, string listId, CancellationToken cancellationToken);
    
    ValueTask<ShoppingList?> GetShoppingList(string userId, string listId, CancellationToken cancellationToken);

    ValueTask<IEnumerable<ShoppingList>> GetShoppingListsForUser(string userId, CancellationToken cancellationToken);
    ValueTask<IEnumerable<ShoppingList>> GetShoppingListsForUser(string userId, bool ordered, int? limit, CancellationToken cancellationToken);
}