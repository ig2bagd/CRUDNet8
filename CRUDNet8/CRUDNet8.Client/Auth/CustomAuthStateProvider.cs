using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace CRUDNet8.Client.Auth;

// https://code-maze.com/authenticationstateprovider-blazor-webassembly/
// https://kpwags.com/posts/2023/07/31/blazor-custom-authentication/
//   https://github.com/kpwags/card-organizer 
// https://kpwags.com/posts/2024/03/22/what-i-learned-blazor-auth-server-prerendering/
public partial class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly BrowserStorageService browserStorageService;

    public CustomAuthStateProvider(BrowserStorageService browserStorageService)
    {
        this.browserStorageService = browserStorageService;
        AuthenticationStateChanged += CustomAuthStateProvider_AuthenticationStateChanged;
    }

    private async void CustomAuthStateProvider_AuthenticationStateChanged(Task<AuthenticationState> task)
    {
        //throw new NotImplementedException();
        var authState = await task;
    }

    // https://github.com/ig2bagd/BlazingPizzaWorkshop/blob/main/BlazingPizza.Client/PersistentAuthenticationStateProvider.cs
    public async override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        //var identity = new ClaimsIdentity("test");

        await Task.Delay(1500);
        var identity = new ClaimsIdentity(new List<Claim> {
            new Claim(ClaimTypes.Name, "Felipe"),
            new Claim(ClaimTypes.Role, "Administrator")
            //new Claim("Token", "DummyValue")
        }, "test");

        return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
    }

    // https://learn.microsoft.com/en-us/aspnet/core/blazor/security/server/?view=aspnetcore-8.0&tabs=visual-studio#notification-about-authentication-state-changes
    public void AuthenticateUser(string userIdentifier)
    {
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, userIdentifier),
        }, "Custom Authentication");

        var user = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    /*
    // https://www.youtube.com/watch?v=arSWQD_I2Zc
    // How to Implement Custom Authentication in Blazor Web Assembly Standalone App - .Net 8 | Abhay Prince
    public async Task LoginAsync(string username, string password)
    {
        // Make Api call to obtain the User info from the server
        ...
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(authState)));
    }
    */
}
