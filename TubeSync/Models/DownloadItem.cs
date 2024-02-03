namespace TubeSync.Models;

public class DownloadItem : ObservableObject
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Duration { get; set; }
    public string AudioCodec { get; set; }
    public string FileSize { get; set; }
    public string FileFormat { get; set; }
    public string VideoCodec { get; set; }
    public string VideoInfo { get; set; }
    public string RemoteUrl { get; set; }
    public string LocalPath { get; set; }
    public string ImageUrl { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    private string _progressText;
    public string ProgressText
    {
        get { return _progressText!; }
        set { SetProperty(ref _progressText, value); }
    }

    [System.Text.Json.Serialization.JsonIgnore]
    private double _progress;
    [System.Text.Json.Serialization.JsonIgnore]
    public double Progress
    {
        get { return _progress; }
        set { SetProperty(ref _progress, value); }
    }

    [System.Text.Json.Serialization.JsonIgnore]
    private bool _isError;
    public bool IsError
    {
        get { return _isError; }
        set { SetProperty(ref _isError, value); }
    }

    [System.Text.Json.Serialization.JsonIgnore]
    private bool _isDownloading;
    public bool IsDownloading
    {
        get { return _isDownloading; }
        set { SetProperty(ref _isDownloading, value); }
    }

    [System.Text.Json.Serialization.JsonIgnore]
    private bool _isCompleted;
    public bool IsCompleted
    {
        get { return _isCompleted; }
        set { SetProperty(ref _isCompleted, value); }
    }

    public DateTime CreatedAt { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    public ImageSource Image => GetImage();

    [NotMapped]
    [System.Text.Json.Serialization.JsonIgnore]
    public CancellationTokenSource? CancellationTokenSource { get; set; }

    private void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            field = value;
            OnPropertyChanged(propertyName);
        }
    }

    private BitmapImage GetImage()
    {
        var bitmap = new BitmapImage();
        bitmap.UriSource = new Uri(ImageUrl!);
        return bitmap;
    }
}
