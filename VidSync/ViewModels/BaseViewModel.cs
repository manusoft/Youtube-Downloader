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
    private bool isAnalyzeError;

    [ObservableProperty]
    private string errorMessage;

    [ObservableProperty, NotifyPropertyChangedFor(nameof(IsNotLoggedIn))]
    private bool isLoggedIn;

    public bool IsNotLoggedIn => !IsLoggedIn;

    [ObservableProperty]
    private string loggedInMessage;

    [ObservableProperty]
    private IReadOnlyList<Cookie> cookies = null!;
}
