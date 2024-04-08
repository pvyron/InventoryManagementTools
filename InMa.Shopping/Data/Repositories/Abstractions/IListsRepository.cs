using InMa.Shopping.Data.Repositories.Models;
using InMa.Shopping.DomainModels;

namespace InMa.Shopping.Data.Repositories.Abstractions;

public interface IListsRepository : IStartupProcess
{
    ValueTask<List> UpdateShoppingList(string userId, UpdateShoppingListData updateData, CancellationToken cancellationToken);

    ValueTask<List> SaveShoppingList(string userId, SaveShoppingListData saveData, CancellationToken cancellationToken);

    ValueTask DeleteShoppingList(string userId, string listId, CancellationToken cancellationToken);
    
    ValueTask<List?> GetShoppingList(string userId, string listId, CancellationToken cancellationToken);

    ValueTask<IEnumerable<List>> GetShoppingListsForUser(string userId, CancellationToken cancellationToken);
}