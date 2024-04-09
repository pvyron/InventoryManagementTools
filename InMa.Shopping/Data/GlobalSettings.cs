namespace InMa.Shopping.Data;

public static class GlobalSettings
{
    public static void Load(IConfiguration configuration)
    {
        MaxOpenShoppingLists = configuration.GetValue<int>("MaxOpenShoppingLists")!;
    }
    
    public static int MaxOpenShoppingLists { get; private set; }
}