using System.Windows;
using Microsoft.Win32;
using FfmpegVideoUtility.Settings;

namespace FfmpegVideoUtility.Views
{
    public partial class SettingsWindow : Window
    {
        public SettingsService SettingsService { get; } = SettingsService.Instance;

        public SettingsWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public AppSettings Settings => SettingsService.Settings;

        private void BrowseFfmpeg(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "FFmpeg|ffmpeg.exe|All Files|*.*"
            };
            if (dialog.ShowDialog() == true)
            {
                Settings.FfmpegPath = dialog.FileName;
                DataContext = null;
                DataContext = this;
            }
        }

        private void BrowseOutput(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Settings.DefaultOutputDirectory = dialog.SelectedPath;
                DataContext = null;
                DataContext = this;
            }
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            SettingsService.Persist();
            DialogResult = true;
        }
    }
}
