using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using YoutubeExplode.Videos;

namespace VidSync.Models;

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

    [JsonIgnore]
    private string _progressText;
    public string ProgressText
    {
        get { return _progressText; }
        set { SetProperty(ref _progressText, value); }
    }

    [JsonIgnore]
    private double _progress;
    [JsonIgnore]
    public double Progress
    {
        get { return _progress; }
        set { SetProperty(ref _progress, value); }
    }

    [JsonIgnore]
    private bool _isError;
    public bool IsError
    {
        get { return _isError; }
        set { SetProperty(ref _isError, value); }
    }

    [JsonIgnore]
    private bool _isDownloading;
    public bool IsDownloading
    {
        get { return _isDownloading; }
        set { SetProperty(ref _isDownloading, value); }
    }

    [JsonIgnore]
    private bool _isCompleted;
    public bool IsCompleted
    {
        get { return _isCompleted; }
        set { SetProperty(ref _isCompleted, value); }
    }

    public DateTime CreatedAt { get; set; }

    [JsonIgnore]
    public ImageSource Image => GetImage();

    [NotMapped]
    [JsonIgnore]
    public CancellationTokenSource CancellationTokenSource { get; set; }

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
        bitmap.UriSource = new Uri(ImageUrl);
        return bitmap;
    }
}
