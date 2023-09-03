using SpeakingTimeTracker;

namespace Tests;

public class IntegrationTest
{
    [Test]
    public async Task Should_successfully_parse_zoom_transcript()
    {
        var filePath = @"sample-transcript.vtt";

        List<TranscriptSegment> segments = await ZoomTranscriptParser.ParseTranscript(File.ReadLinesAsync(filePath));

        Assert.AreEqual(13, segments.Count);
    }
}