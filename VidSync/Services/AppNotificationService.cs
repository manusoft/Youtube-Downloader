namespace VidSync.Notifications;

public class AppNotificationService : IAppNotificationService
{
    private readonly INavigationService _navigationService;

    public AppNotificationService(INavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    ~AppNotificationService()
    {
        Unregister();
    }

    public void Initialize()
    {
        AppNotificationManager.Default.NotificationInvoked += OnNotificationInvoked;

        AppNotificationManager.Default.Register();
    }

    public void OnNotificationInvoked(AppNotificationManager sender, AppNotificationActivatedEventArgs args)
    {
        // TODO: Handle notification invocations when your app is already running.

        if (ParseArguments(args.Argument)["action"] == "Rate")
        {
            App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                try
                {
                    Process.Start("explorer.exe", "https://www.manojbabu.in");
                }
                catch (Exception)
                {
                }
            });
        }

        if (ParseArguments(args.Argument)["action"] == "Report")
        {
            App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                try
                {
                    Process.Start("explorer.exe", "https://github.com/manusoft/Youtube-Downloader/issues/new");
                }
                catch (Exception)
                {
                }
            });
        }


        //App.MainWindow.DispatcherQueue.TryEnqueue(() =>
        //{
        //    App.MainWindow.ShowMessageDialogAsync("Download is completed! Rate this App please...", "Vidsync");

        //    App.MainWindow.BringToFront();
        //});
    }

    public bool Show(string payload)
    {
        var appNotification = new AppNotification(payload);

        AppNotificationManager.Default.Show(appNotification);

        return appNotification.Id != 0;
    }

    public NameValueCollection ParseArguments(string arguments)
    {
        return HttpUtility.ParseQueryString(arguments);
    }

    public void Unregister()
    {
        AppNotificationManager.Default.Unregister();
    }
}
