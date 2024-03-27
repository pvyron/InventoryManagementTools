namespace InMa.ShoppingList.DomainExtensions;

public static class EnumerableExtensions
{
    public static async ValueTask<List<T>> ToListAsync<T>(this ValueTask<IEnumerable<T>> input)
    {
        return (await input).ToList();
    }
}