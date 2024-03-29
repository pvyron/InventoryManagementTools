using Blazored.LocalStorage;

namespace InMa.ShoppingList.Components.Services;

public sealed class KeyCheckingService
{
    private const string KeyName = "AuthKey";
    
    private readonly ILocalStorageService _localStorageService;
    private readonly string? _correctKey;
    
    public KeyCheckingService(IConfiguration configuration, ILocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
        _correctKey = configuration.GetValue<string>(KeyName);
    }

    public async Task SaveKey(string key, CancellationToken cancellationToken)
    {
        await _localStorageService.SetItemAsStringAsync(KeyName, key, cancellationToken);
    }

    public async Task<bool> IsAuthorized(CancellationToken cancellationToken)
    {
        var suppliedKey = await _localStorageService.GetItemAsStringAsync(KeyName, cancellationToken);
        
        if (suppliedKey is null || _correctKey is null)
            return false;

        return suppliedKey == _correctKey;
    }
}