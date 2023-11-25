using Microsoft.UI.Xaml.Controls;

using VidSync.ViewModels;

namespace VidSync.Views;

public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel { get; }

    public MainPage()
    {
        ViewModel = App.GetService<MainViewModel>();
        InitializeComponent();
    }
}
