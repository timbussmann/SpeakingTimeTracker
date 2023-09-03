using System.Globalization;

namespace SpeakingTimeTracker;

public class ZoomTranscriptParser
{
    static readonly string[] WebVttTimestampFormats = new[]
    {
        "mm:ss.fff",
        "HH:mm:ss.fff"
    };

    public static async Task<List<TranscriptSegment>> ParseTranscript(IAsyncEnumerable<string> transcript)
    {
        var asyncEnum = transcript.GetAsyncEnumerator();
        await asyncEnum.MoveNextAsync();

        if (asyncEnum.Current != "WEBVTT")
        {
            throw new ArgumentException("transcript input doesn't seem to be a valid WebVTT file. Expected first line to be 'WEBVTT' according to https://www.w3.org/TR/webvtt1/.");
        }

        var segments = new List<TranscriptSegment>();
        while (await asyncEnum.MoveNextAsync())
        {
            var segment = await ParseSegment(asyncEnum);
            segments.Add(segment);
        }

        await asyncEnum.DisposeAsync();
        return segments;
    }

    public static async Task<TranscriptSegment> ParseSegment(IAsyncEnumerator<string> asyncEnum)
    {
        var segment = new TranscriptSegment();
        while (asyncEnum.Current == null || !asyncEnum.Current.Contains("-->"))
        {
            await asyncEnum.MoveNextAsync();
        }
        var timingLine = asyncEnum.Current;
        var timingParts = timingLine.Split("-->");
        segment.StartTime = TimeOnly.ParseExact(timingParts[0].Trim(), WebVttTimestampFormats);
        segment.EndTime = TimeOnly.ParseExact(timingParts[1].Trim(), WebVttTimestampFormats);
        await asyncEnum.MoveNextAsync();

        while (!string.IsNullOrEmpty(asyncEnum.Current))
        {
            segment.Text += asyncEnum.Current;
            await asyncEnum.MoveNextAsync();
        }

        var speakerNameIndex = segment.Text.IndexOf(':');
        if (speakerNameIndex > 0)
        {
            segment.SpeakerName = segment.Text.Substring(0, speakerNameIndex);
        }

        return segment;
    }
}