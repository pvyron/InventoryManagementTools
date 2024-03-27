using InMa.ShoppingList.DomainModels;

namespace InMa.ShoppingList.DomainExtensions;

public static class EnumExtensions
{
    public static ListItemBoughtStatus ToListItemBoughtStatus(this bool? input)
    {
        return input switch
        {
            null => ListItemBoughtStatus.None,
            true => ListItemBoughtStatus.Bought,
            false => ListItemBoughtStatus.NotBought
        };
    }

    public static bool? ToNullableBool(this ListItemBoughtStatus input)
    {
        return input switch
        {
            ListItemBoughtStatus.Bought => true,
            ListItemBoughtStatus.NotBought => false,
            _ => null
        };
    }
}