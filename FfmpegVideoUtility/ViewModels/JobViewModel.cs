using System;
using FfmpegVideoUtility.Models;

namespace FfmpegVideoUtility.ViewModels
{
    public class JobViewModel : ViewModelBase
    {
        private Guid _id;
        private string _description = string.Empty;
        private JobStatus _status;
        private int _progress;

        public JobViewModel()
        {
        }

        public JobViewModel(VideoJob job)
        {
            UpdateFrom(job);
        }

        public Guid Id
        {
            get => _id;
            private set => SetProperty(ref _id, value);
        }

        public string Description
        {
            get => _description;
            private set => SetProperty(ref _description, value);
        }

        public JobStatus Status
        {
            get => _status;
            private set => SetProperty(ref _status, value);
        }

        public int Progress
        {
            get => _progress;
            private set => SetProperty(ref _progress, value);
        }

        public void UpdateFrom(VideoJob job)
        {
            Id = job.Id;
            Description = job.Description;
            Status = job.Status;
            Progress = job.Progress;
        }
    }
}
