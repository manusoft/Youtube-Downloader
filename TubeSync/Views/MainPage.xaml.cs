using Windows.ApplicationModel.DataTransfer;

namespace TubeSync.Views;

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

        SearchBox.Text = string.Empty;

        await analyzeDialog.ShowAsync();
    }

    protected override async void OnGotFocus(RoutedEventArgs e)
    {
        var package = Clipboard.GetContent();

        if (SearchBox.Text == string.Empty)
        {            
            if (package.Contains(StandardDataFormats.Text))
            {
                var text = await package.GetTextAsync();

                if (text.Contains("youtube.com"))
                {
                    SearchBox.Text = text;
                    Clipboard.Clear();
                }
            }

        }
    }
}
