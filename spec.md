# FFmpeg Video Utility GUI -- Specification

## Purpose

Create a Windows desktop application in C#/.NET that provides a simple
GUI over FFmpeg for common video tasks.

## Core Features (MVP)

-   **Single Video Transcoder**
    -   Input video → standardized MP4 output
    -   Options: resolution, format, quality (CRF/preset)
-   **Thumbnail Generator**
    -   Extract frame at user-defined timestamp
-   **Clip & GIF Maker**
    -   Extract video segment
    -   Optional GIF creation
-   **Batch Video Normalizer**
    -   Normalize resolution/format for entire folders
    -   Simple job queue with status per file

## UI Design (WPF / .NET 8)

-   Main window with three tabs:
    -   *Transcode*
    -   *Clip & GIF*
    -   *Batch*
-   Input selectors (file/folder)
-   Options panels for each workflow
-   Output section with progress + log access
-   Settings dialog:
    -   FFmpeg path
    -   Default output directory
    -   Max concurrent jobs

## Architecture

### Components

-   **FFmpeg Service**
    -   Methods: Transcode, GenerateThumbnail, CreateClip, CreateGif
    -   Uses `Process` to run FFmpeg, captures logs
-   **Job Model**
    -   Tracks type, params, status, progress, error logs
-   **Job Queue**
    -   Sequential or limited parallel job execution
    -   Reports events to UI
-   **Presets**
    -   JSON-based configuration for common output profiles

## Logging & Error Handling

-   Per-job log files stored in `%AppData%/AppName/logs`
-   Errors surfaced in UI with "View Log" button

## Development Phases

1.  Core console FFmpeg wrapper
2.  GUI skeleton (WPF)
3.  Job queue + batch processing
4.  Presets + UI polish
5.  Optional agentic coding enhancements

## Non-Functional Requirements

-   Windows 10+
-   .NET 8 / WPF
-   Async operations to keep UI responsive
