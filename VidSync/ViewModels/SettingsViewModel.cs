using System.Net;

namespace VidSync.ViewModels;

public partial class SettingsViewModel : BaseViewModel, INavigationAware
{
    private readonly IThemeSelectorService _themeSelectorService;

    [ObservableProperty]
    private ElementTheme _elementTheme;

    [ObservableProperty]
    private string _versionDescription;

    public ICommand SwitchThemeCommand { get; }

    public SettingsViewModel(IThemeSelectorService themeSelectorService)
    {
        _themeSelectorService = themeSelectorService;
        _elementTheme = _themeSelectorService.Theme;
        _versionDescription = GetVersionDescription();

        SwitchThemeCommand = new RelayCommand<ElementTheme>(
            async (param) =>
            {
                if (ElementTheme != param)
                {
                    ElementTheme = param;
                    await _themeSelectorService.SetThemeAsync(param);
                }
            });
    }

    private static string GetVersionDescription()
    {
        Version version;

        if (RuntimeHelper.IsMSIX)
        {
            var packageVersion = Package.Current.Id.Version;

            version = new(packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision);
        }
        else
        {
            version = Assembly.GetExecutingAssembly().GetName().Version!;
        }

        return $"{"AppDisplayName".GetLocalized()} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
    }

    [RelayCommand]
    private void GotoBack()
    {
        if (NavigationService.CanGoBack)
            NavigationService.GoBack();
    }

    [RelayCommand]
    private void GotoLoginPage()
    {
        NavigationService.NavigateTo(typeof(LoginViewModel).FullName!.ToString(), null);
    }

    public void OnNavigatedTo(object parameter)
    {
        if(parameter is not null)
        {
            var cookies = parameter as IReadOnlyList<Cookie>;

            if (cookies!.Count > 0)
            {
                IsLoggedIn = true;
                LoggedInMessage = "You're already signed in. Dive into the app and make the most of your experience!";
            }
            else
            {
                IsLoggedIn = false;
                LoggedInMessage = "You're not signed in. Sign in to explore videos and channels tailored to your interests.";
            }
        }
    }

    public void OnNavigatedFrom()
    {
        //throw new NotImplementedException();
    }
}
