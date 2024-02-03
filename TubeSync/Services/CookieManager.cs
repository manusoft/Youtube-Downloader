using System.Net;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace TubeSync.Services;

public class CookieManager : ICookieManager
{
    private string CookiesFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "cookies.json");

    public async Task<List<Cookie>> LoadCookiesAsync()
    {
        try
        {
            if (File.Exists(CookiesFileName))
            {
                string json = await File.ReadAllTextAsync(CookiesFileName);
                return JsonSerializer.Deserialize<List<Cookie>>(json)!;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading cookies: {ex.Message}");
        }

        return new List<Cookie>();
    }

    public async Task SaveCookiesAsync(List<Cookie> cookies)
    {
        try
        {
            string json = JsonSerializer.Serialize(cookies);
            await File.WriteAllTextAsync(CookiesFileName, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving cookies: {ex.Message}");
        }
    }

    public bool DeleteCookiesAsync()
    {
        try
        {
            if (File.Exists(CookiesFileName))
            {
                File.Delete(CookiesFileName);    
                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error delete cookies: {ex.Message}");
        }

        return false;
    }
}
