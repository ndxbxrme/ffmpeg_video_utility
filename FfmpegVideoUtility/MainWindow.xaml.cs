using System.Windows;
using FfmpegVideoUtility.ViewModels;

namespace FfmpegVideoUtility
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            if (DataContext is MainViewModel vm)
            {
                vm.Initialize();
            }
        }
    }
}
