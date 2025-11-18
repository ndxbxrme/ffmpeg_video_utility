using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FfmpegVideoUtility.Models;
using FfmpegVideoUtility.Settings;

namespace FfmpegVideoUtility.Services;

public class JobQueue
{
    private readonly ConcurrentQueue<VideoJob> _pendingJobs = new();
    private readonly List<VideoJob> _activeJobs = new();
    private readonly SemaphoreSlim _semaphore;
    private readonly FfmpegService _ffmpegService;
    private readonly SettingsService _settingsService;

    public event EventHandler<VideoJob>? JobUpdated;

    public JobQueue(FfmpegService service, SettingsService settingsService)
    {
        _ffmpegService = service;
        _settingsService = settingsService;
        _semaphore = new SemaphoreSlim(_settingsService.Settings.MaxConcurrentJobs);
    }

    public IEnumerable<VideoJob> ActiveJobs => _activeJobs;

    public void Enqueue(VideoJob job)
    {
        _pendingJobs.Enqueue(job);
        _ = ProcessQueueAsync();
    }

    private async Task ProcessQueueAsync()
    {
        while (_pendingJobs.TryDequeue(out var job))
        {
            await _semaphore.WaitAsync();
            _activeJobs.Add(job);
            _ = Task.Run(async () => await ExecuteJobAsync(job));
        }
    }

    private async Task ExecuteJobAsync(VideoJob job)
    {
        try
        {
            job.Status = JobStatus.Running;
            RaiseJobUpdated(job);
            var progress = new Progress<int>(value =>
            {
                job.Progress = Math.Min(100, job.Progress + value);
                RaiseJobUpdated(job);
            });

            switch (job.Type)
            {
                case JobType.Transcode:
                case JobType.BatchTranscode:
                    await _ffmpegService.RunTranscodeAsync(job, progress, job.Cancellation.Token);
                    break;
                case JobType.Thumbnail:
                    await _ffmpegService.RunThumbnailAsync(job, job.Cancellation.Token);
                    break;
                case JobType.Clip:
                    await _ffmpegService.RunClipAsync(job, job.Cancellation.Token);
                    break;
                case JobType.Gif:
                    await _ffmpegService.RunGifAsync(job, job.Cancellation.Token);
                    break;
            }

            job.Status = JobStatus.Completed;
            job.Progress = 100;
        }
        catch (OperationCanceledException)
        {
            job.Status = JobStatus.Cancelled;
        }
        catch (Exception ex)
        {
            job.Status = JobStatus.Failed;
            job.Options["error"] = ex.Message;
        }
        finally
        {
            _activeJobs.Remove(job);
            _semaphore.Release();
            RaiseJobUpdated(job);
        }
    }

    private void RaiseJobUpdated(VideoJob job) => JobUpdated?.Invoke(this, job);
}
