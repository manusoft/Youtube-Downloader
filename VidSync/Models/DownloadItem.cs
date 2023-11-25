namespace VidSync.Models;

public class DownloadItem
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string AudioCodec { get; set; }
    //public Bitrate Bitrate { get; set; }
    //public Container Container { get; set; }
    //public FileSize FileSize { get; set; }
    public string VideoCodec { get; set; }
    //public VideoQuality VideoQuality { get; set; }
    //public Resolution Resolution { get; set; }
    public string RemoteUrl { get; set; }
    public string LocalPath { get; set; }
    public string Image { get; set; }
    public string Progress { get; set; } = "0%";
    public bool IsDownloading { get; set; } = false;
    public bool IsCompleted { get; set; } = false;
    public DateTime CreatedAt { get; set; }
}
