using CommunityToolkit.Mvvm.ComponentModel;
using VidSync.Contracts.Services;
using VidSync.Services;

namespace VidSync.ViewModels;

public partial class BaseViewModel : ObservableRecipient
{
    public readonly INavigationService NavigationService;

    public BaseViewModel()
    {
       NavigationService = App.GetService<NavigationService>();
    }

    [ObservableProperty]
    private bool isAnalyzing;

    [ObservableProperty]
    private bool isAnalyzed;
}
