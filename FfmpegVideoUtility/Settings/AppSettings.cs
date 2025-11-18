namespace FfmpegVideoUtility.Settings;

public class AppSettings
{
    public string FfmpegPath { get; set; } = string.Empty;
    public string DefaultOutputDirectory { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
    public int MaxConcurrentJobs { get; set; } = 2;
}
