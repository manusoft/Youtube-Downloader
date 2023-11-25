using CommunityToolkit.Mvvm.ComponentModel;
using VidSync.Services;

namespace VidSync.ViewModels;

public class BaseViewModel : ObservableRecipient
{
    public readonly NavigationService NavigationService;

    public BaseViewModel()
    {
       NavigationService = App.GetService<NavigationService>();
    }
}
