using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Media;
using YoutubeExplode.Common;
using YoutubeExplode.Videos.Streams;

namespace VidSync.Models;

public class DownloadItem : ObservableObject
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Duration { get; set; }
    public string AudioCodec { get; set; }
    public FileSize FileSize { get; set; }
    public Bitrate Bitrate { get; set; }
    public Container Container { get; set; }
    public string VideoCodec { get; set; }
    public VideoQuality VideoQuality { get; set; }
    public Resolution Resolution { get; set; }
    public string RemoteUrl { get; set; }
    public string LocalPath { get; set; }
    public ImageSource Image { get; set; }
    public string Progress { get; set; } = "0%";
    public bool IsDownloading { get; set; } = false;
    public bool IsCompleted { get; set; } = false;
    public DateTime CreatedAt { get; set; }

    public string VideoInfo => $"{Resolution.Width}x{Resolution.Height} {VideoQuality.Framerate}fps";
    public string FileMegaByte => $"{Math.Round(FileSize.MegaBytes, 1)}MB";


}
