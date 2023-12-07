namespace VidSync.ViewModels;

public partial class BaseViewModel : ObservableRecipient
{
    public readonly INavigationService NavigationService;
    //public readonly IAppNotificationService AppNotificationService;

    public BaseViewModel()
    {
        NavigationService = App.GetService<INavigationService>();
        //AppNotificationService = App.GetService<IAppNotificationService>();
    }

    [ObservableProperty]
    private bool isAnalyzing;

    [ObservableProperty]
    private bool isAnalyzed;
}
