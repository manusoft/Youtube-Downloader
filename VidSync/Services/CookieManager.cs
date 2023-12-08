namespace VidSync.Services;

public class CookieManager : ICookieManager
{
    private string CookiesFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "cookies.json");

    public List<System.Net.Cookie> LoadCookies()
    {
        try
        {
            if (File.Exists(CookiesFileName))
            {
                string json = File.ReadAllText(CookiesFileName);
                return System.Text.Json.JsonSerializer.Deserialize<List<System.Net.Cookie>>(json)!;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading cookies: {ex.Message}");
        }

        return new List<System.Net.Cookie>();
    }

    public void SaveCookies(List<System.Net.Cookie> cookies)
    {
        try
        {
            string json = System.Text.Json.JsonSerializer.Serialize(cookies);
            File.WriteAllText(CookiesFileName, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving cookies: {ex.Message}");
        }
    }
}
