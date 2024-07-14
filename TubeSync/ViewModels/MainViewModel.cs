using AngleSharp.Common;
using System.Net;

namespace TubeSync.ViewModels;

public partial class MainViewModel : BaseViewModel, INavigationAware
{
    private int maxConcurrentDownloads = 2;
    private SemaphoreSlim downloadSemaphore = new SemaphoreSlim(2);

    public MainViewModel()
    {
        InitializeAsync();
    }

    private async void InitializeAsync()
    {
        await LoadDownloadListAsync();
        await CheckDownloadsAsync();
    }

    private async Task CheckDownloadsAsync()
    {
        try
        {
            foreach (var item in DownloadItems)
            {
                if (item.ProgressText == "COMPLETED" || item.ProgressText == "100%")
                {
                    item.IsError = false;
                    item.IsDownloading = false;
                    item.IsCompleted = true;
                    item.ProgressText = "COMPLETED";
                }
                else
                {
                    item.IsError = true;
                    item.IsDownloading = false;
                    item.IsCompleted = false;
                    item.ProgressText = "PENDING";
                }

            }

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public async Task AnalyzeVideoLinkAsync()
    {
        if (string.IsNullOrEmpty(VideoLink) || IsAnalyzing) return;

        try
        {
            Qualities.Clear();
            IsAnalyzing = true;
            IsAnalyzeError = false;
            IsAnalyzed = false;

            var bitmap = new BitmapImage();

            var youtube = new YoutubeClient(Cookies);
            var video = await youtube.Videos.GetAsync(VideoLink);
            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(VideoLink);

            StreamInfo = streamManifest.Streams;

            // Get highest quality muxed stream
            //StreamInfo = streamManifest.GetMuxedStreams();
        

            //var highQ = StreamInfo.GetWithHighestVideoQuality();

            //foreach (var item in streams)
            //{
            //    Qualities.Add(item.VideoQuality.Label);
            //}

            foreach (var item in StreamInfo)
            {
                if(!Qualities.Contains(item.ToString()!))
                    Qualities.Add(item.ToString()!);
            }

            ThumbnailUrl = video.Thumbnails.LastOrDefault()!.Url;

            bitmap.UriSource = new Uri(ThumbnailUrl);

            VideoId = video.Id;
            VideoTitle = video.Title;
            VideoDuration = video.Duration!.Value.ToString();
            VideoThumbnail = bitmap;
            SelectedQuality = StreamInfo.TryGetWithHighestBitrate()!.ToString()!;
            IsAnalyzed = true;
        }
        catch (Exception ex)
        {
            IsAnalyzeError = true;
            ErrorMessage = ex.Message;
            IsAnalyzed = false;
        }
        finally
        {
            IsAnalyzing = false;
        }
    }

    [RelayCommand]
    private async Task AddDownloadItem()
    {
        if (DownloadItems.Any(x => x.Id == VideoId))
        {
            ShowMessageBox("Download item already exist!!", "TubeSync");
            return;
        }

        try
        {
            if(StreamInfo != null)
            {
                foreach (var item in StreamInfo)
                {
                  
                    if(item.ToString() == SelectedQuality)
                    {
                        DownloadItem downloadItem = new DownloadItem()
                        {
                            Id = VideoId,
                            Title = VideoTitle,
                            Duration = VideoDuration,
                            ImageUrl = ThumbnailUrl,
                            Bitrate = item.Bitrate.ToString(),
                            FileFormat = item.Container.Name,
                            FileSize = $"{Math.Round(item.Size.MegaBytes, 1)}MB",
                            IsAudioOnly = item.Container.IsAudioOnly,
                            VideoInfo = SelectedQuality,
                            RemoteUrl = item.Url,
                            LocalPath = AppContants.DownloadPath,
                            CreatedAt = DateTime.Now,
                            CancellationTokenSource = new CancellationTokenSource(),
                        };

                        if (!DownloadItems.Any(x => x.Id == VideoId))
                            DownloadItems.Insert(0, downloadItem); //Add(downloadItem);
                    }
                }
            }           

            SaveDownloadList();

            await StartDownloadAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    [RelayCommand]
    private async Task StartDownloadAsync()
    {
        try
        {
            // Set the maximum number of parallel tasks
            int maxParallelTasks = GetMaxProcessCount(); // Change this number based on your preference
            SemaphoreSlim semaphoreSlim = new SemaphoreSlim(maxParallelTasks);

            // Create a list to store the running tasks
            List<Task> runningTasks = new List<Task>();

            foreach (var item in DownloadItems.Where(item => !item.IsCompleted && !item.IsDownloading))
            {
                // Wait for a slot in the semaphore (up to the maximum)
                await semaphoreSlim.WaitAsync();

                // Start the task and add it to the list
                Task convertTask = StartDownloadItemAsync(item);
                runningTasks.Add(convertTask);

                // When the task completes, release the semaphore slot
                convertTask.ContinueWith(_ => semaphoreSlim.Release());
            }

            // Wait for all running tasks to complete
            await Task.WhenAll(runningTasks);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            // Release the semaphore when all downloads are complete or an exception occurs
            downloadSemaphore.Release();
        }
    }

    private int GetMaxProcessCount()
    {
        int count;

        // Read value from local settings
        object maxParallelTasksValue = ApplicationData.Current.LocalSettings.Values["MaxParallelTasks"];

        // Check if the value is present and cast it to an int
        if (maxParallelTasksValue != null && maxParallelTasksValue is int maxParallelTasks)
        {
            // Now 'maxParallelTasks' contains the value you stored in local settings
            // Use it as needed in your application logic
            count = maxParallelTasks;
        }
        else
        {
            // Save value to local settings
            ApplicationData.Current.LocalSettings.Values["MaxParallelTasks"] = 3;
            count = 3;
        }

        return count;
    }

    private async Task StartDownloadItemAsync(DownloadItem item)
    {
        try
        {
            item.IsError = false;
            item.IsDownloading = true;
            item.ProgressText = "DOWNLOADING ...";
            item.CancellationTokenSource = new CancellationTokenSource();

            await DownloadItemAsync(item, item.CancellationTokenSource.Token);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private async Task DownloadItemAsync(DownloadItem download, CancellationToken cancellationToken)
    {
        try
        {
            ProgressChanged = 0;

            var downloadFileUrl = download.RemoteUrl;

            var saveFileName = PathExtension.ConvertToValidFileName(download.Title);

            var destinationFilePath = $"{AppContants.DownloadPath}\\{saveFileName}.{download.FileFormat}";

            download.LocalPath = destinationFilePath;

            using (var client = new DownloadService(downloadFileUrl!, destinationFilePath))
            {
                client.CancellationToken = cancellationToken;

                client.ProgressChanged += (totalFileSize, totalBytesDownloaded, progressPercentage) =>
                {
                    var dispatcherQueue = App.MainWindow.DispatcherQueue;

                    if (dispatcherQueue != null)
                    {
                        dispatcherQueue.TryEnqueue(() =>
                        {
                            Debug.WriteLine($"{progressPercentage}% ({totalBytesDownloaded}/{totalFileSize})");
                            
                            if (progressPercentage == 100)
                                download.ProgressText = $"COMPLETED";
                            else
                                download.ProgressText = $"{progressPercentage}%";

                            download.Progress = (double)progressPercentage!;
                            ProgressChanged = Math.Round((double)progressPercentage / 100, 2);
                        });
                    }

                    // Check for cancellation and stop the download if necessary
                    if (download.CancellationTokenSource!.Token.IsCancellationRequested)
                    {
                        //The cancellation will be handled by exceptions
                    }
                };

                try
                {
                    // Pass CancellationToken to the StartDownload method
                    await client.StartDownload(cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    // Handle the cancellation exception
                    Debug.WriteLine("Download Cancelled");
                    return;
                }
            }

            await Task.CompletedTask;

            download.IsDownloading = false;
            download.IsCompleted = true;
            download.IsError = false;
            download.ProgressText = "COMPLETED";
            SaveDownloadList();
            App.GetService<IAppNotificationService>().Show(string.Format("AppNotificationDownloadComplete".GetLocalized(), AppContext.BaseDirectory));
        }
        catch (Exception ex)
        {
            download.IsDownloading = false;
            download.IsCompleted = false;
            download.IsError = download.CancellationTokenSource!.Token.IsCancellationRequested; // Set IsError based on cancellation
            SaveDownloadList();
            Debug.WriteLine(ex.Message);
            App.GetService<IAppNotificationService>().Show(string.Format("AppNotificationDownloadError".GetLocalized(), AppContext.BaseDirectory));
        }
    }

    [RelayCommand]
    private void StopDownload(DownloadItem item)
    {
        try
        {
            if (item.IsDownloading)
            {
                // Cancel the download operation associated with the DownloadItem
                item.CancellationTokenSource?.Cancel();

                // Update the item's properties to reflect the cancellation
                item.IsDownloading = false;
                item.IsCompleted = false;
                item.IsError = true;
                item.ProgressText = "CANCELLED";
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    [RelayCommand]
    private void DeleteItem(DownloadItem item)
    {
        if (item is null) return;

        try
        {
            DownloadItems.Remove(item);
            SaveDownloadList();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    [RelayCommand]
    private void OpenFolder()
    {
        try
        {
            Process.Start("explorer.exe", AppContants.DownloadPath);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    //[RelayCommand]
    //private async Task PauseDownload(string id)
    //{
    //    var downloadItem = GetDownloadItem(id);
    //    if (downloadItem != null)
    //    {
    //        //downloadItem.IsPaused = true;
    //        downloadItem.CancellationTokenSource.Cancel();
    //    }
    //}

    //[RelayCommand]
    //async Task ResumeDownload(string id)
    //{
    //    var downloadItem = GetDownloadItem(id);
    //    if (downloadItem != null)
    //    {
    //        //downloadItem.IsPaused = false;
    //        downloadItem.CancellationTokenSource = new CancellationTokenSource();
    //        //await StartDownloadAsync(downloadItem);
    //    }
    //}

    [RelayCommand]
    private void GotoSettingsPage()
    {
        NavigationService.NavigateTo(typeof(SettingsViewModel).FullName!.ToString(), Cookies);
    }

    [RelayCommand]
    private void GotoLoginPage()
    {
        if (Cookies.Count == 0)
        {
            NavigationService.NavigateTo(typeof(LoginViewModel).FullName!.ToString(), null);
        }
        else
        {
            ShowMessageBox("You're already signed in. Dive into the app and make the most of your experience!", "TubeSync");
        }
    }

    private DownloadItem? GetDownloadItem(string id)
    {
        return DownloadItems.Where(item => item.Id == id).FirstOrDefault();
    }

    private void SaveDownloadList()
    {
        try
        {
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "tubesync.json");
            var jsonString = System.Text.Json.JsonSerializer.Serialize(DownloadItems);
            File.WriteAllText(filePath, jsonString);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    private async Task LoadDownloadListAsync()
    {
        try
        {
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "tubesync.json");

            if (File.Exists(filePath))
            {
                var jsonString = await File.ReadAllTextAsync(filePath);
                DownloadItems = System.Text.Json.JsonSerializer.Deserialize<ObservableCollection<DownloadItem>>(jsonString)!;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    public void ShowMessageBox(string content, string title)
    {
        App.MainWindow.DispatcherQueue.TryEnqueue(() =>
        {
            App.MainWindow.ShowMessageDialogAsync(content, title);
            App.MainWindow.BringToFront();
        });
    }

    public async void OnNavigatedTo(object parameter)
    {
        await GetCookiesAsync();
    }

    public void OnNavigatedFrom()
    {
        //throw new NotImplementedException();
    }

    private async Task GetCookiesAsync()
    {
        try
        {
            // Load cookies
            List<Cookie> storedCookies = await CookieManager.LoadCookiesAsync();

            // Use the cookies (e.g., set them in the WebView2 control)
            Cookies = storedCookies;

            if (Cookies.Count > 0)
                IsLoggedIn = true;
            else
                IsLoggedIn = false;

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public ObservableCollection<DownloadItem> DownloadItems { get; set; } = new ObservableCollection<DownloadItem>();
    public ObservableCollection<string> Qualities { get; set; } = new ObservableCollection<string>();

    [ObservableProperty]
    private string videoId = string.Empty;

    [ObservableProperty]
    private DownloadItem selectedItem = null!;

    [ObservableProperty]
    private string videoLink = string.Empty;

    [ObservableProperty]
    private string videoTitle = string.Empty;

    [ObservableProperty]
    private ImageSource videoThumbnail = null!;

    [ObservableProperty]
    private string thumbnailUrl = null!;

    [ObservableProperty]
    private string videoDuration = string.Empty;

    [ObservableProperty]
    private IEnumerable<IStreamInfo>? streamInfo;

    [ObservableProperty]
    private string selectedQuality = "720p";

    [ObservableProperty]
    private double progressChanged;

    [ObservableProperty]
    private string localPath = AppContants.DownloadPath;

}
