using Microsoft.JSInterop;

namespace MonsterKampf.Services;

/// <summary>
/// Einfacher LocalStorage-Dienst für Blazor WebAssembly via JS Interop
/// </summary>
public class LocalStorageService
{
    private readonly IJSRuntime _js;

    public LocalStorageService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task<string?> GetItemAsync(string key)
    {
        try
        {
            return await _js.InvokeAsync<string?>("localStorage.getItem", key);
        }
        catch
        {
            return null;
        }
    }

    public async Task SetItemAsync(string key, string value)
    {
        try
        {
            await _js.InvokeVoidAsync("localStorage.setItem", key, value);
        }
        catch { }
    }

    public async Task RemoveItemAsync(string key)
    {
        try
        {
            await _js.InvokeVoidAsync("localStorage.removeItem", key);
        }
        catch { }
    }
}
