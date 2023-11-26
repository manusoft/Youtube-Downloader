using Microsoft.UI.Xaml.Controls;
using System.Diagnostics;
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

    private void ButtonOpenFolder_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        try
        {
            Process.Start("explorer.exe", ViewModel.LocalPath);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    private void ButtonDeleteItem_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if(ViewModel.SelectedItem != null)
        {
            ViewModel.DeleteItemCommand.Execute(ViewModel.SelectedItem);
        }
    }

    private void ButtonRetry_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ViewModel.StartDownloadCommand.Execute(null);
    }

    private void ButtonCancel_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (ViewModel.SelectedItem != null)
        {
            ViewModel.StopDownloadCommand.Execute(ViewModel.SelectedItem);
        }
    }
}
