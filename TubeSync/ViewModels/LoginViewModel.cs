namespace TubeSync.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    public LoginViewModel()
    {
    }

    [RelayCommand]
    private void GotoBack()
    {
        if (NavigationService.CanGoBack)
            NavigationService.GoBack();
    }
}
