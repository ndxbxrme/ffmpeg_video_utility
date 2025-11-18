using System.Windows;
using FfmpegVideoUtility.Settings;

namespace FfmpegVideoUtility;

public partial class App : Application
{
    private SettingsService? _settingsService;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        _settingsService = SettingsService.Instance;
        _settingsService.Initialize();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _settingsService?.Persist();
        base.OnExit(e);
    }
}
