using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FfmpegVideoUtility.Models;
using FfmpegVideoUtility.Settings;

namespace FfmpegVideoUtility.Services
{
    public class FfmpegService
    {
        private readonly SettingsService _settingsService;

        public FfmpegService(SettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public async Task RunTranscodeAsync(VideoJob job, IProgress<int>? progress = null, CancellationToken cancellationToken = default)
        {
            var preset = job.Options.TryGetValue("preset", out var presetValue) ? presetValue : "medium";
            var crf = job.Options.TryGetValue("crf", out var crfValue) ? crfValue : "20";
            var resolution = job.Options.TryGetValue("resolution", out var resolutionValue) ? resolutionValue : "1280x720";
            var args = $"-y -i \"{job.InputPath}\" -vf scale={resolution} -c:v libx264 -preset {preset} -crf {crf} -c:a aac \"{job.OutputPath}\"";
            await RunFfmpegAsync(job, args, progress, cancellationToken);
        }

        public async Task RunThumbnailAsync(VideoJob job, CancellationToken cancellationToken = default)
        {
            var timestamp = job.Options.GetValueOrDefault("timestamp", "00:00:01");
            var args = $"-y -ss {timestamp} -i \"{job.InputPath}\" -vframes 1 \"{job.OutputPath}\"";
            await RunFfmpegAsync(job, args, null, cancellationToken);
        }

        public async Task RunClipAsync(VideoJob job, CancellationToken cancellationToken = default)
        {
            var start = job.Options.GetValueOrDefault("start", "00:00:00");
            var end = job.Options.GetValueOrDefault("end", "00:00:10");
            var args = $"-y -ss {start} -to {end} -i \"{job.InputPath}\" -c copy \"{job.OutputPath}\"";
            await RunFfmpegAsync(job, args, null, cancellationToken);
        }

        public async Task RunGifAsync(VideoJob job, CancellationToken cancellationToken = default)
        {
            var start = job.Options.GetValueOrDefault("start", "00:00:00");
            var end = job.Options.GetValueOrDefault("end", "00:00:05");
            var fps = job.Options.GetValueOrDefault("fps", "12");
            var args = $"-y -ss {start} -to {end} -i \"{job.InputPath}\" -vf fps={fps},scale=640:-1:flags=lanczos -gifflags -offsetting \"{job.OutputPath}\"";
            await RunFfmpegAsync(job, args, null, cancellationToken);
        }

        private async Task RunFfmpegAsync(VideoJob job, string arguments, IProgress<int>? progress, CancellationToken cancellationToken)
        {
            var ffmpegPath = _settingsService.Settings.FfmpegPath;
            if (string.IsNullOrWhiteSpace(ffmpegPath))
            {
                throw new InvalidOperationException("FFmpeg path is not configured.");
            }

            var logDirectory = _settingsService.GetLogDirectory();
            Directory.CreateDirectory(logDirectory);
            var logFile = Path.Combine(logDirectory, $"{job.Id}.log");
            job.LogFilePath = logFile;

            var startInfo = new ProcessStartInfo
            {
                FileName = ffmpegPath,
                Arguments = arguments,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = new Process { StartInfo = startInfo, EnableRaisingEvents = true };
            var builder = new StringBuilder();

            process.OutputDataReceived += (_, args) =>
            {
                if (args.Data != null)
                {
                    builder.AppendLine(args.Data);
                }
            };

            process.ErrorDataReceived += (_, args) =>
            {
                if (args.Data != null)
                {
                    builder.AppendLine(args.Data);
                }
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            while (!process.HasExited)
            {
                await Task.Delay(500, cancellationToken);
                progress?.Report(10);
            }

            await process.WaitForExitAsync(cancellationToken);
            await File.WriteAllTextAsync(logFile, builder.ToString(), cancellationToken);
            if (process.ExitCode != 0)
            {
                throw new InvalidOperationException($"FFmpeg exited with code {process.ExitCode}");
            }
        }
    }
}
