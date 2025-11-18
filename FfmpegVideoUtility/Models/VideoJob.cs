using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FfmpegVideoUtility.Models;

public class VideoJob
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Description { get; set; } = string.Empty;
    public JobType Type { get; set; }
    public JobStatus Status { get; set; } = JobStatus.Pending;
    public int Progress { get; set; }
    public string InputPath { get; set; } = string.Empty;
    public string OutputPath { get; set; } = string.Empty;
    public Dictionary<string, string> Options { get; set; } = new();
    public string? LogFilePath { get; set; }

    [JsonIgnore]
    public CancellationTokenSource Cancellation { get; } = new();
}
