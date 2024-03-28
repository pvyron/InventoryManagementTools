namespace InMa.ShoppingList.Components.Services;

public sealed class KeyBearingService
{
    private readonly string? _correctKey;
    
    public KeyBearingService(IConfiguration configuration)
    {
        _correctKey = configuration.GetValue<string>("AuthKey");
    }
    
    public string? Key { get; private set; }

    public void SaveKey(string key)
    {
        Key = key;
    }

    public bool IsAuthorized()
    {
        if (Key is null || _correctKey is null)
            return false;

        return Key == _correctKey;
    }
}