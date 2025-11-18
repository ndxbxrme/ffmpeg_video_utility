using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FfmpegVideoUtility.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        protected void OnPropertyChanged(string? propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
