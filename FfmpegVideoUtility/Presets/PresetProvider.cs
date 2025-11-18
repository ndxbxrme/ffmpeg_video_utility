using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using FfmpegVideoUtility.Models;

namespace FfmpegVideoUtility.Presets
{
    public class PresetProvider
    {
        private readonly string _presetPath;

        public PresetProvider(string presetPath)
        {
            _presetPath = presetPath;
        }

        public ObservableCollection<Preset> LoadPresets()
        {
            if (!File.Exists(_presetPath))
            {
                return new ObservableCollection<Preset>(GetDefaultPresets());
            }

            var json = File.ReadAllText(_presetPath);
            var presets = JsonSerializer.Deserialize<IEnumerable<Preset>>(json);
            return new ObservableCollection<Preset>(presets ?? GetDefaultPresets());
        }

        private IEnumerable<Preset> GetDefaultPresets() => new[]
        {
            new Preset { Name = "Web 1080p", Resolution = "1920x1080", Crf = 20, SpeedPreset = "medium" },
            new Preset { Name = "Web 720p", Resolution = "1280x720", Crf = 22, SpeedPreset = "faster" },
            new Preset { Name = "Mobile", Resolution = "854x480", Crf = 24, SpeedPreset = "fast" }
        };
    }
}
