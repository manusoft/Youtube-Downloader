using Microsoft.Web.WebView2.Core;

namespace TubeSync.Views;

public sealed partial class LoginPage : Page
{
    private Uri loginUri = new Uri("https://accounts.google.com/ServiceLogin?continue=https%3A%2F%2Fwww.youtube.com");
    private bool loggedIn = false;

    public LoginViewModel ViewModel { get; }

    public LoginPage()
    {
        ViewModel = App.GetService<LoginViewModel>();
        InitializeComponent();
        webView2.Source = loginUri;
    }

    private async void webView2_NavigationCompleted(WebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
    {
        if (args.IsSuccess)
        {
            // Check if there are cookies
            var cookiesTask = await webView2.CoreWebView2.CookieManager.GetCookiesAsync("http://www.youtube.com");

            if (cookiesTask.Count > 0)
            {
                // The user is logged in
                Console.WriteLine("User is logged in.");
                var cookieCollection = new List<System.Net.Cookie>();

                foreach (var cookieString in cookiesTask)
                {
                        var cookie = new System.Net.Cookie
                        {
                            Name = cookieString.Name,
                            Value = cookieString.Value,
                            Domain = webView2.Source.Host,
                            Path = webView2.Source.AbsolutePath
                        };
                        cookieCollection.Add(cookie);
                }

                loggedIn = true;

                // Update ViewModel with the collected cookies
                await ViewModel.CookieManager.SaveCookiesAsync(cookieCollection);

                ViewModel.GotoBackCommand.Execute(null);
            }
            else
            {
                loggedIn = false;
                // The user is not logged in
                Console.WriteLine("User is not logged in.");

                // Check if the current source is different from the login URI
                if (webView2.Source is null)
                {
                    // Set the source to the login page only if it's not the current source
                    webView2.Source = loginUri;
                }
            }
        }
    }

    private async void webView2_WebMessageReceived(WebView2 sender, CoreWebView2WebMessageReceivedEventArgs args)
    {
        try
        {
            // Check if the message contains cookie information
            if (args.WebMessageAsJson is not null)
            {
                var message = args.WebMessageAsJson.ToString();

                // Parse the JSON string into a JsonDocument
                using (JsonDocument jsonDocument = JsonDocument.Parse(message))
                {
                    // Check if the "cookies" property is present
                    if (jsonDocument.RootElement.TryGetProperty("cookies", out var cookiesElement))
                    {
                        string cookies = cookiesElement.GetString()!;

                        // Adjust the cookie parsing based on your actual cookie format and requirements
                        if (!string.IsNullOrEmpty(cookies) && cookies.StartsWith("SID="))
                        {
                            string[] cookieParts = cookies.Split(';');

                            // Convert the cookies to System.Net.Cookie objects
                            var cookieCollection = new List<System.Net.Cookie>();

                            foreach (var cookieString in cookieParts)
                            {
                                var cookieKeyValue = cookieString.Trim().Split('=');
                                if (cookieKeyValue.Length == 2)
                                {
                                    var cookie = new System.Net.Cookie
                                    {
                                        Name = cookieKeyValue[0],
                                        Value = cookieKeyValue[1],
                                        Domain = webView2.Source.Host,
                                        Path = webView2.Source.AbsolutePath
                                    };
                                    cookieCollection.Add(cookie);
                                }
                            }

                            loggedIn = true;

                            // Update ViewModel with the collected cookies
                            await ViewModel.CookieManager.SaveCookiesAsync(cookieCollection);

                            ViewModel.GotoBackCommand.Execute(null);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private void webView2_NavigationStarting(WebView2 sender, CoreWebView2NavigationStartingEventArgs args)
    {
        Console.WriteLine(args.Uri.ToString());
    }
}
