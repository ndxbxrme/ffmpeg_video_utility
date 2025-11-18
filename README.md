# FFmpeg Video Utility

A Windows desktop demo app that wraps FFmpeg workflows (transcoding, clipping, GIF creation, thumbnail generation, and batch normalization) behind a friendly WPF/.NET 8 user interface.

## Requirements

- Windows 10 or later
- .NET SDK 5.0 with WPF workloads installed
- FFmpeg binary available locally (download from [ffmpeg.org](https://ffmpeg.org/download.html))

## Getting Started

1. Restore dependencies and build the solution:
   ```bash
   dotnet build FfmpegVideoUtility/FfmpegVideoUtility.csproj
   ```
2. Run the WPF app:
   ```bash
   dotnet run --project FfmpegVideoUtility/FfmpegVideoUtility.csproj
   ```
3. Open the **File → Settings** dialog inside the app and configure:
   - Path to `ffmpeg.exe`
   - Default output directory
   - Maximum number of concurrent jobs

## Features

- **Transcode tab** – load a video, pick a preset, tweak CRF/preset, and queue a single MP4 export.
- **Clip & GIF tab** – trim clips, export GIFs, or generate thumbnails at any timestamp.
- **Batch tab** – scan a folder, normalize all files to a consistent resolution/format with a queue per file.
- **Job queue** – tracks job progress, stores per-job logs under `%AppData%/FfmpegVideoUtility/logs`, and exposes quick access via the status bar.
- **Presets** – JSON-based presets (`Presets/presets.json`) for common output profiles that can be extended without recompiling.

## Project Layout

```
FfmpegVideoUtility/
├── Models/          # Job + preset data structures
├── Services/        # FFmpeg process wrapper and job queue
├── Settings/        # Settings persistence and dialog
├── ViewModels/      # MVVM layer for each tab
├── Views/           # WPF windows (main UI + settings)
├── Presets/         # Sample preset definitions
└── Themes/          # Shared resource dictionary
```

This repository is meant for demo/educational purposes; tailor the commands and UI to fit your production scenario.
