using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using FfmpegVideoUtility.Models;
using FfmpegVideoUtility.Services;
using FfmpegVideoUtility.Utilities;

namespace FfmpegVideoUtility.ViewModels;

public class BatchViewModel : ViewModelBase
{
    private readonly JobQueue _jobQueue;

    private string _inputFolder = string.Empty;
    private string _outputFolder = string.Empty;
    private string _selectedResolution = "1920x1080";

    public ObservableCollection<string> Resolutions { get; } = new() { "3840x2160", "1920x1080", "1280x720", "854x480" };
    public ObservableCollection<string> PendingFiles { get; } = new();

    public RelayCommand BrowseInputCommand { get; }
    public RelayCommand BrowseOutputCommand { get; }
    public RelayCommand ScanCommand { get; }
    public RelayCommand StartBatchCommand { get; }

    public string InputFolder
    {
        get => _inputFolder;
        set => SetProperty(ref _inputFolder, value);
    }

    public string OutputFolder
    {
        get => _outputFolder;
        set => SetProperty(ref _outputFolder, value);
    }

    public string SelectedResolution
    {
        get => _selectedResolution;
        set => SetProperty(ref _selectedResolution, value);
    }

    public BatchViewModel(JobQueue queue)
    {
        _jobQueue = queue;
        BrowseInputCommand = new RelayCommand(_ => BrowseInput());
        BrowseOutputCommand = new RelayCommand(_ => BrowseOutput());
        ScanCommand = new RelayCommand(_ => ScanFolder(), _ => Directory.Exists(InputFolder));
        StartBatchCommand = new RelayCommand(_ => StartBatch(), _ => PendingFiles.Any());
    }

    private void BrowseInput()
    {
        var dialog = new System.Windows.Forms.FolderBrowserDialog();
        if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            InputFolder = dialog.SelectedPath;
        }
    }

    private void BrowseOutput()
    {
        var dialog = new System.Windows.Forms.FolderBrowserDialog();
        if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            OutputFolder = dialog.SelectedPath;
        }
    }

    private void ScanFolder()
    {
        if (!Directory.Exists(InputFolder))
        {
            MessageBox.Show("Input folder does not exist.");
            return;
        }

        var files = Directory.EnumerateFiles(InputFolder)
            .Where(file => new[] { ".mp4", ".mov", ".mkv", ".avi" }.Contains(Path.GetExtension(file), StringComparer.OrdinalIgnoreCase));
        PendingFiles.Clear();
        foreach (var file in files)
        {
            PendingFiles.Add(file);
        }
        MessageBox.Show($"Discovered {PendingFiles.Count} files.");
    }

    private void StartBatch()
    {
        if (string.IsNullOrWhiteSpace(OutputFolder))
        {
            MessageBox.Show("Please choose an output folder.");
            return;
        }

        foreach (var file in PendingFiles)
        {
            var output = Path.Combine(OutputFolder, Path.GetFileNameWithoutExtension(file) + "_normalized.mp4");
            var job = new VideoJob
            {
                Type = JobType.BatchTranscode,
                InputPath = file,
                OutputPath = output,
                Description = $"Batch {Path.GetFileName(file)}"
            };
            job.Options["resolution"] = SelectedResolution;
            job.Options["crf"] = "22";
            job.Options["preset"] = "faster";
            _jobQueue.Enqueue(job);
        }

        MessageBox.Show("Batch jobs queued.");
    }
}
