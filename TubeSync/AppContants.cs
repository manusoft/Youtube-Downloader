namespace TubeSync;

public static class AppContants
{
    public static string DownloadPath => Environment.GetEnvironmentVariable("USERPROFILE") + @"\" + @"Downloads\";
}
