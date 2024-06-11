using System.Security.Claims;
using InMa.Shopping.DomainEnums;

namespace InMa.Shopping.Components.Account;

public sealed class IdentityClaimsProvider
{
    private const string AUTH_CLAIM_TYPE_NAME = "pvyronAuth";
    
    public static IEnumerable<Claim> InitializeCustomClaims()
    {
        var authorizationClaim = new Claim(AUTH_CLAIM_TYPE_NAME, ((byte)PvyronAuthorizations.ShoppingListsComplete).ToString());
        var authorizationClaim3 = new Claim(AUTH_CLAIM_TYPE_NAME, ((byte)PvyronAuthorizations.ShoppingLists).ToString());

        return [authorizationClaim, authorizationClaim3];
    }
    
    
}