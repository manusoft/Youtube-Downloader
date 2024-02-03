namespace TubeSync.Views;

public sealed partial class AnalyzeDialog : ContentDialog
{
    public MainViewModel ViewModel { get; }

    public AnalyzeDialog()
    {
        ViewModel = App.GetService<MainViewModel>();

        //var buttonStyle = new Style(typeof(Button));
        //buttonStyle.Setters.Add(new Setter(Control.BackgroundProperty, Colors.Red));
        //buttonStyle.Setters.Add(new Setter(Control.ForegroundProperty, Colors.White));
        //this.PrimaryButtonStyle = buttonStyle;

        this.InitializeComponent();
    }

    private async void ContentDialog_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
    {
        await ViewModel.AnalyzeVideoLinkAsync();
    }
}
