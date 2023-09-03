namespace SpeakingTimeTracker;

public record TranscriptSegment
{
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public TimeSpan Duration => EndTime - StartTime;
    public string Text { get; set; } = string.Empty;
    public string? SpeakerName { get; set; }
}