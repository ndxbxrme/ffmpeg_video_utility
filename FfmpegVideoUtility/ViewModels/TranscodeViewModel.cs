using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using FfmpegVideoUtility.Models;
using FfmpegVideoUtility.Presets;
using FfmpegVideoUtility.Services;
using FfmpegVideoUtility.Settings;
using FfmpegVideoUtility.Utilities;

namespace FfmpegVideoUtility.ViewModels
{
    public class TranscodeViewModel : ViewModelBase
    {
        private readonly JobQueue _jobQueue;
        private readonly PresetProvider _presetProvider;
        private readonly SettingsService _settingsService = SettingsService.Instance;

    private string _inputFile = string.Empty;
    private string _outputFile = string.Empty;
    private Preset? _selectedPreset;
    private string _selectedResolution = "1920x1080";
    private string _selectedSpeedPreset = "medium";
    private int _crf = 20;

    public ObservableCollection<Preset> Presets { get; } = new();
    public ObservableCollection<string> Resolutions { get; } = new() { "3840x2160", "2560x1440", "1920x1080", "1280x720", "854x480" };
    public ObservableCollection<string> SpeedPresets { get; } = new() { "veryslow", "slow", "medium", "fast", "faster" };

    public RelayCommand BrowseInputCommand { get; }
    public RelayCommand BrowseOutputCommand { get; }
    public RelayCommand StartTranscodeCommand { get; }

    public string InputFile
    {
        get => _inputFile;
        set => SetProperty(ref _inputFile, value);
    }

    public string OutputFile
    {
        get => _outputFile;
        set => SetProperty(ref _outputFile, value);
    }

    public Preset? SelectedPreset
    {
        get => _selectedPreset;
        set
        {
            if (value != null)
            {
                _crf = value.Crf;
                _selectedResolution = value.Resolution;
                _selectedSpeedPreset = value.SpeedPreset;
                SetProperty(ref _selectedPreset, value);
                OnPropertyChanged(nameof(Crf));
                OnPropertyChanged(nameof(SelectedResolution));
                OnPropertyChanged(nameof(SelectedSpeedPreset));
                return;
            }
            SetProperty(ref _selectedPreset, value);
        }
    }

    public string SelectedResolution
    {
        get => _selectedResolution;
        set => SetProperty(ref _selectedResolution, value);
    }

    public string SelectedSpeedPreset
    {
        get => _selectedSpeedPreset;
        set => SetProperty(ref _selectedSpeedPreset, value);
    }

    public int Crf
    {
        get => _crf;
        set => SetProperty(ref _crf, value);
    }

    public TranscodeViewModel(JobQueue queue, PresetProvider presets)
    {
        _jobQueue = queue;
        _presetProvider = presets;
        BrowseInputCommand = new RelayCommand(_ => BrowseInput());
        BrowseOutputCommand = new RelayCommand(_ => BrowseOutput(), _ => !string.IsNullOrWhiteSpace(InputFile));
        StartTranscodeCommand = new RelayCommand(_ => StartTranscode(), _ => CanStartTranscode());
    }

    public void LoadPresets()
    {
        Presets.Clear();
        foreach (var preset in _presetProvider.LoadPresets())
        {
            Presets.Add(preset);
        }
        SelectedPreset = Presets.FirstOrDefault();
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
            if (string.IsNullOrWhiteSpace(OutputFile))
            {
                OutputFile = Path.Combine(_settingsService.Settings.DefaultOutputDirectory, Path.GetFileNameWithoutExtension(InputFile) + ".mp4");
            }
        }
    }

    private void BrowseOutput()
    {
        var dialog = new SaveFileDialog
        {
            Filter = "MP4 File|*.mp4",
            FileName = Path.GetFileNameWithoutExtension(InputFile) + ".mp4"
        };
        if (dialog.ShowDialog() == true)
        {
            OutputFile = dialog.FileName;
        }
    }

    private bool CanStartTranscode() => !string.IsNullOrWhiteSpace(InputFile) && !string.IsNullOrWhiteSpace(OutputFile);

        private void StartTranscode()
        {
            var job = new VideoJob
            {
                Type = JobType.Transcode,
                InputPath = InputFile,
                OutputPath = OutputFile,
                Description = $"Transcode {Path.GetFileName(InputFile)}"
            };
            job.Options["preset"] = SelectedSpeedPreset;
            job.Options["crf"] = Crf.ToString();
            job.Options["resolution"] = SelectedResolution;

            _jobQueue.Enqueue(job);
            MessageBox.Show("Transcode job added to queue.");
        }
    }
}
