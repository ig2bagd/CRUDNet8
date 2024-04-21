using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace CRUDNet8.Client.Auth;

public class DummyAuthStateProvider : AuthenticationStateProvider
{
    public async override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        //var identity = new ClaimsIdentity("test");

        //await Task.Delay(3000);
        var identity = new ClaimsIdentity(new List<Claim> {
            new Claim(ClaimTypes.Name, "Felipe")
        }, "test");
        return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
    }
}
