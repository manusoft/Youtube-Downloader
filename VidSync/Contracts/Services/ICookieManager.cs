using System.Net;

namespace VidSync.Contracts.Services
{
    public interface ICookieManager
    {
        Task<List<Cookie>> LoadCookiesAsync();
        Task SaveCookiesAsync(List<Cookie> cookies);
        bool DeleteCookiesAsync();
    }
}