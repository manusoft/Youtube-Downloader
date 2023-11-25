using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Collections.ObjectModel;
using VidSync.Models;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace VidSync.ViewModels;

public partial class MainViewModel : BaseViewModel
{    
    public MainViewModel()
    {
        DownloadItem download = new DownloadItem()
        {
            Id = Guid.NewGuid().ToString(),
            Title = "തമാശക്കാരന്റെ മൂന്നാംകിട ചിന്തയ്ക്ക്മൂലാഭിവാദ്യങ്ങൾ I On Air 23.11.2023",
            Duration = "00:00:00",
        };

        DownloadItems.Add(download);
    }

    public async Task AnalyzeVideoLinkAsync()
    {
        if (string.IsNullOrEmpty(VideoLink)) return;
        if (IsAnalyzing) return;

        try
        {
            Qualities.Clear();
            IsAnalyzing = true;
            IsAnalyzed = false;

            var bitmap = new BitmapImage();

            var youtube = new YoutubeClient();
            var video = await youtube.Videos.GetAsync("https://www.youtube.com/watch?v=8jWIYoKFIoU");
            var streamManifest = await youtube.Videos.Streams.GetManifestAsync("https://www.youtube.com/watch?v=8jWIYoKFIoU");

            // Get highest quality muxed stream
            StreamInfo = streamManifest.GetMuxedStreams();

            foreach (var item in StreamInfo.ToList())
            {
                Qualities.Add(item.VideoQuality.Label);
            }

            bitmap.UriSource = new Uri(video.Thumbnails.LastOrDefault()!.Url);

            VideoTitle = video.Title;
            VideoDuration = video.Duration!.Value.ToString();
            VideoThumbnail = bitmap;
            SelectedQuality = "720p";
            IsAnalyzed = true;
        }
        catch (Exception)
        {
            IsAnalyzed = false;
        }
        finally
        {
            IsAnalyzing = false;
        }
    }

    [RelayCommand]
    private void AddDownloadItem()
    {
        try
        {
            var videoPlayerStream = StreamInfo.First(video => video.VideoQuality.Label == SelectedQuality);

            DownloadItem downloadItem = new DownloadItem()
            {
                Id = Guid.NewGuid().ToString(),
                Title = VideoTitle,
                Duration = VideoDuration,
                Image = VideoThumbnail,
                AudioCodec = videoPlayerStream.AudioCodec,
                Bitrate = videoPlayerStream.Bitrate,
                Container = videoPlayerStream.Container,
                FileSize = videoPlayerStream.Size,
                VideoCodec = videoPlayerStream.VideoCodec,
                VideoQuality = videoPlayerStream.VideoQuality,
                Resolution = videoPlayerStream.VideoResolution,
                RemoteUrl = videoPlayerStream.Url,
                LocalPath = LocalPath,
                CreatedAt = DateTime.Now,
            };

            DownloadItems.Add(downloadItem);
        }
        catch (Exception)
        {
            throw;
        }       
    }

    [RelayCommand]
    private void GotoSettingsPage()
    {
        NavigationService.NavigateTo(typeof(SettingsViewModel).FullName!.ToString(), null);
    }

    public ObservableCollection<DownloadItem> DownloadItems { get; set; } = new ObservableCollection<DownloadItem>();
    public ObservableCollection<string> Qualities { get; set; } = new ObservableCollection<string>();

    [ObservableProperty]
    private string videoLink = string.Empty;

    [ObservableProperty]
    private string videoTitle = string.Empty;

    [ObservableProperty]
    private ImageSource videoThumbnail = null!;

    [ObservableProperty]
    private string videoDuration = string.Empty;

    [ObservableProperty]
    private string localPath = @"c:\downloads\";

    [ObservableProperty]
    private IEnumerable<MuxedStreamInfo> streamInfo;

    [ObservableProperty]
    private string selectedQuality = "720p";

}
