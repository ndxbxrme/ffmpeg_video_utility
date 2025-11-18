using System.IO;
using System.Windows;
using Microsoft.Win32;
using FfmpegVideoUtility.Models;
using FfmpegVideoUtility.Services;
using FfmpegVideoUtility.Utilities;

namespace FfmpegVideoUtility.ViewModels;

public class ClipGifViewModel : ViewModelBase
{
    private readonly JobQueue _jobQueue;

    private string _inputFile = string.Empty;
    private string _startTimestamp = "00:00:00";
    private string _endTimestamp = "00:00:05";
    private string _thumbnailTimestamp = "00:00:01";

    public RelayCommand BrowseInputCommand { get; }
    public RelayCommand CreateClipCommand { get; }
    public RelayCommand CreateGifCommand { get; }
    public RelayCommand GenerateThumbnailCommand { get; }

    public string InputFile
    {
        get => _inputFile;
        set => SetProperty(ref _inputFile, value);
    }

    public string StartTimestamp
    {
        get => _startTimestamp;
        set => SetProperty(ref _startTimestamp, value);
    }

    public string EndTimestamp
    {
        get => _endTimestamp;
        set => SetProperty(ref _endTimestamp, value);
    }

    public string ThumbnailTimestamp
    {
        get => _thumbnailTimestamp;
        set => SetProperty(ref _thumbnailTimestamp, value);
    }

    public ClipGifViewModel(JobQueue queue)
    {
        _jobQueue = queue;
        BrowseInputCommand = new RelayCommand(_ => BrowseInput());
        CreateClipCommand = new RelayCommand(_ => CreateClip(), _ => CanCreate());
        CreateGifCommand = new RelayCommand(_ => CreateGif(), _ => CanCreate());
        GenerateThumbnailCommand = new RelayCommand(_ => GenerateThumbnail(), _ => CanCreate());
    }

    private void BrowseInput()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Video Files|*.mp4;*.mov;*.mkv;*.avi|All Files|*.*"
        };
        if (dialog.ShowDialog() == true)
        {
            InputFile = dialog.FileName;
        }
    }

    private bool CanCreate() => !string.IsNullOrWhiteSpace(InputFile);

    private void CreateClip()
    {
        var output = Path.Combine(Path.GetDirectoryName(InputFile) ?? string.Empty, Path.GetFileNameWithoutExtension(InputFile) + "_clip.mp4");
        var job = new VideoJob
        {
            Type = JobType.Clip,
            InputPath = InputFile,
            OutputPath = output,
            Description = $"Clip {Path.GetFileName(InputFile)}"
        };
        job.Options["start"] = StartTimestamp;
        job.Options["end"] = EndTimestamp;
        _jobQueue.Enqueue(job);
        MessageBox.Show("Clip job added.");
    }

    private void CreateGif()
    {
        var output = Path.Combine(Path.GetDirectoryName(InputFile) ?? string.Empty, Path.GetFileNameWithoutExtension(InputFile) + ".gif");
        var job = new VideoJob
        {
            Type = JobType.Gif,
            InputPath = InputFile,
            OutputPath = output,
            Description = $"GIF {Path.GetFileName(InputFile)}"
        };
        job.Options["start"] = StartTimestamp;
        job.Options["end"] = EndTimestamp;
        job.Options["fps"] = "12";
        _jobQueue.Enqueue(job);
        MessageBox.Show("GIF job added.");
    }

    private void GenerateThumbnail()
    {
        var output = Path.Combine(Path.GetDirectoryName(InputFile) ?? string.Empty, Path.GetFileNameWithoutExtension(InputFile) + "_thumb.jpg");
        var job = new VideoJob
        {
            Type = JobType.Thumbnail,
            InputPath = InputFile,
            OutputPath = output,
            Description = $"Thumbnail {Path.GetFileName(InputFile)}"
        };
        job.Options["timestamp"] = ThumbnailTimestamp;
        _jobQueue.Enqueue(job);
        MessageBox.Show("Thumbnail job added.");
    }
}
