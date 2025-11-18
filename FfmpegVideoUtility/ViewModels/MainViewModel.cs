using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using FfmpegVideoUtility.Models;
using FfmpegVideoUtility.Presets;
using FfmpegVideoUtility.Services;
using FfmpegVideoUtility.Settings;
using FfmpegVideoUtility.Utilities;
using FfmpegVideoUtility.Views;

namespace FfmpegVideoUtility.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly SettingsService _settingsService;
        private readonly FfmpegService _ffmpegService;
        private readonly JobQueue _jobQueue;

    private string _queueStatus = "Ready";

    public ObservableCollection<JobViewModel> Jobs { get; } = new();

    public TranscodeViewModel Transcode { get; }
    public ClipGifViewModel Clip { get; }
    public BatchViewModel Batch { get; }

    public RelayCommand OpenSettingsCommand { get; }
    public RelayCommand ExitCommand { get; }
    public RelayCommand OpenLogsCommand { get; }

    public string QueueStatus
    {
        get => _queueStatus;
        set => SetProperty(ref _queueStatus, value);
    }

    public MainViewModel()
    {
        _settingsService = SettingsService.Instance;
        _ffmpegService = new FfmpegService(_settingsService);
        _jobQueue = new JobQueue(_ffmpegService, _settingsService);
        _jobQueue.JobUpdated += OnJobUpdated;

        var presetPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Presets", "presets.json");
        var presetProvider = new PresetProvider(presetPath);

        Transcode = new TranscodeViewModel(_jobQueue, presetProvider);
        Clip = new ClipGifViewModel(_jobQueue);
        Batch = new BatchViewModel(_jobQueue);

        OpenSettingsCommand = new RelayCommand(_ => OpenSettings());
        ExitCommand = new RelayCommand(_ => Application.Current.Shutdown());
        OpenLogsCommand = new RelayCommand(_ => OpenLogs());
    }

    public void Initialize()
    {
        Transcode.LoadPresets();
    }

    private void OpenSettings()
    {
        var window = new SettingsWindow();
        window.Owner = Application.Current.MainWindow;
        window.ShowDialog();
        QueueStatus = $"Settings updated at {DateTime.Now:HH:mm:ss}";
    }

    private void OpenLogs()
    {
        var logDir = _settingsService.GetLogDirectory();
        Directory.CreateDirectory(logDir);
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "explorer",
                Arguments = logDir,
                UseShellExecute = true
            });
        }
        catch
        {
            MessageBox.Show($"Logs located at {logDir}");
        }
    }

    private void OnJobUpdated(object? sender, VideoJob job)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            var existing = Jobs.FirstOrDefault(j => j.Id == job.Id);
            if (existing == null)
            {
                existing = new JobViewModel(job);
                Jobs.Add(existing);
            }
            else
            {
                existing.UpdateFrom(job);
            }
            QueueStatus = $"{Jobs.Count(j => j.Status == JobStatus.Running)} running / {Jobs.Count} total";
        });
    }
    }
}
