using Microsoft.JSInterop;
using System.Text.Json;

namespace CRUDNet8.Client.Auth;

// https://github.com/csharpfritz/BlazingPizzaWorkshop/blob/main/BlazingPizza.Client/OrderState.cs
public class BrowserStorageService
{
    /*
     * localStorage. | sessionStorage.setItem('key', 'value');
     * localStorage. | sessionStorage.getItem('key');
     * localStorage. | sessionStorage.removeItem('key');
     * localStorage. | sessionStorage.clear();
     */
    private const string StorageType = "localStorage";
    private readonly IJSRuntime jSRuntime;

    public BrowserStorageService(IJSRuntime jSRuntime)
    {
        this.jSRuntime = jSRuntime;
    }

    public async Task SaveToStorage<TData>(string key, TData value)
    {
        var serializeData = Serialize(value);
        await jSRuntime.InvokeVoidAsync($"{StorageType}.setItem", key, serializeData);
    }

    public async Task<TData?> GetFromStorage<TData>(string key)
    {
        var serializedData = await jSRuntime.InvokeAsync<string?>($"{StorageType}.getItem", key);
        return Deserialize<TData?>(serializedData);
    }

    public async Task RemoveFromStorage(string key)
    {
        await jSRuntime.InvokeVoidAsync($"{StorageType}.removeItem", key);
    }

    private static readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions();

    private static string Serialize<TData>(TData data) => JsonSerializer.Serialize(data, jsonSerializerOptions);

    private static TData? Deserialize<TData>(string? jsonData)
    {
        if (!string.IsNullOrWhiteSpace(jsonData))
        {
            return JsonSerializer.Deserialize<TData>(jsonData, jsonSerializerOptions);
        }
        return default;
    }
}
