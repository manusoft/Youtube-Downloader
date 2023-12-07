using System.Net;

namespace VidSync.ViewModels;

public partial class BaseViewModel : ObservableRecipient
{
    public readonly INavigationService NavigationService;
    public readonly ICookieManager CookieManager;

    public BaseViewModel()
    {
        NavigationService = App.GetService<INavigationService>();
        CookieManager = App.GetService<ICookieManager>();
    }

    [ObservableProperty]
    private bool isAnalyzing;

    [ObservableProperty]
    private bool isAnalyzed;

    [ObservableProperty]
    private IReadOnlyList<Cookie> cookies;
}
