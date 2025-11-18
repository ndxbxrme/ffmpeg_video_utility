using System;
using System.IO;
using System.Text.Json;

namespace FfmpegVideoUtility.Settings
{
    public class SettingsService
    {
        private static readonly Lazy<SettingsService> _instance = new(() => new SettingsService());
        public static SettingsService Instance => _instance.Value;

        private readonly string _appDirectory;
        private readonly string _settingsFile;

        public AppSettings Settings { get; private set; } = new();

        private SettingsService()
        {
            _appDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FfmpegVideoUtility");
            _settingsFile = Path.Combine(_appDirectory, "settings.json");
        }

        public void Initialize()
        {
            Directory.CreateDirectory(_appDirectory);
            if (File.Exists(_settingsFile))
            {
                var json = File.ReadAllText(_settingsFile);
                Settings = JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
            }
            else
            {
                Persist();
            }
        }

        public void Persist()
        {
            Directory.CreateDirectory(_appDirectory);
            var json = JsonSerializer.Serialize(Settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_settingsFile, json);
        }

        public string GetLogDirectory() => Path.Combine(_appDirectory, "logs");
    }
}
