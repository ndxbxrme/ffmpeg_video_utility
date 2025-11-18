namespace FfmpegVideoUtility.Models;

public class Preset
{
    public string Name { get; set; } = string.Empty;
    public string VideoCodec { get; set; } = "libx264";
    public string AudioCodec { get; set; } = "aac";
    public string Container { get; set; } = "mp4";
    public string Resolution { get; set; } = "1920x1080";
    public int Crf { get; set; } = 20;
    public string SpeedPreset { get; set; } = "medium";
}
