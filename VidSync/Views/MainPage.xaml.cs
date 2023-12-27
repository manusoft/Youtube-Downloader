namespace VidSync.Views;

public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel { get; }

    public MainPage()
    {
        ViewModel = App.GetService<MainViewModel>();
        InitializeComponent();
    }

    private async void SearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        var queryText = args.QueryText.Trim();

        if (string.IsNullOrWhiteSpace(queryText)) return;

        ContentDialog analyzeDialog = new AnalyzeDialog();
        analyzeDialog.XamlRoot = this.XamlRoot;

        ViewModel.VideoLink = queryText;

        var result = await analyzeDialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
        }
    }
}
