using InMa.Shopping.Data.Repositories.Models;
using InMa.Shopping.DomainModels;

namespace InMa.Shopping.Data.Repositories.Abstractions;

public interface IListsRepository : IStartupProcess
{
    ValueTask<List> UpdateOpenShoppingList(string userId, UpdateShoppingListData updateData, CancellationToken cancellationToken);

    ValueTask<List> SaveOpenShoppingList(string userId, SaveShoppingListData saveData, CancellationToken cancellationToken);

    ValueTask DeleteOpenShoppingList(string userId, string listId, CancellationToken cancellationToken);
    
    ValueTask<List?> GetOpenShoppingList(string userId, string listId, CancellationToken cancellationToken);

    ValueTask<IEnumerable<List>> GetOpenShoppingListsForUser(string userId, CancellationToken cancellationToken);
}