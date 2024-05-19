using Microsoft.JSInterop;

namespace CRUDNet8.Client.Services;

public class SessionStorage
{
    private readonly IJSRuntime jSRuntime;

    public SessionStorage(IJSRuntime jSRuntime)
    {
        this.jSRuntime = jSRuntime;
    }

    public async Task SaveAsync(string key, string value)
    {
        await jSRuntime.InvokeVoidAsync("window.sessionStorage.setItem", key, value);
    }

    public async Task<string?> GetAsync(string key)
    {
        return await jSRuntime.InvokeAsync<string?>("window.sessionStorage.getItem", key);
    }

    public async Task RemoveAsync(string key)
    {
        await jSRuntime.InvokeVoidAsync("window.sessionStorage.removeItem", key);
    }

}
