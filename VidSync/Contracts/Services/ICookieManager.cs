using System.Net;

namespace VidSync.Contracts.Services
{
    public interface ICookieManager
    {
        List<Cookie> LoadCookies();
        void SaveCookies(List<Cookie> cookies);
    }
}