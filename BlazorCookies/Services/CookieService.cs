using Microsoft.JSInterop;
using System.Diagnostics;
using System.Text.Json;

namespace BlazorCookies.Services;

public class CookieService
{
    private readonly IJSRuntime _jsRuntime;

    public CookieService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task SetCookieAsync(string key, string value)
    {
        try
        {
            // Inject JS function once (safe even if repeated)
            await _jsRuntime.InvokeVoidAsync("eval", @"
                window.setCookieViaFetch = async function (url, data) {
                    const response = await fetch(url, {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json'
                        },
                        credentials: 'include',
                        body: JSON.stringify(data)
                    });
                    return await response.text();
                };
            ");

            var requestData = new { Key = key, Value = value };

            var response = await _jsRuntime.InvokeAsync<string>(
                "setCookieViaFetch",
                "/api/cookie/save",
                requestData
            );

        }
        catch (Exception ex)
        {
            throw new Exception("Error setting cookie", ex);
        }
    }

    public async Task<string> ReadCookieAsync(string key)
    {
        try
        {
            // Inject JS if not already done
            await _jsRuntime.InvokeVoidAsync("eval", @"
                window.blazorFetch = async function(url, method, data) {
                    const options = {
                        method: method,
                        headers: { 'Content-Type': 'application/json' },
                        credentials: 'include'
                    };
                    if (method === 'POST') {
                        options.body = JSON.stringify(data);
                    }
                    const res = await fetch(url, options);
                    return await res.text();
                };
            ");

            var requestData = new { Key = key };

            var result = await _jsRuntime.InvokeAsync<string>(
                "blazorFetch",
                "/api/cookie/read-cookie",
                "POST",
                requestData
            );

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception("Error reading cookie", ex);
        }
    }

}